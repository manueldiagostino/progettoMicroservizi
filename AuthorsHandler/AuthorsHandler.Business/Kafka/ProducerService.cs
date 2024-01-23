using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AuthorsHandler.Repository.Abstraction;
using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Services;
using GlobalUtility.Kafka.Config;
using GlobalUtility.Kafka.Model;

namespace AuthorsHandler.Business.Kafka;
public class ProducerService : ProducerService<KafkaTopicsOutput> {
	public ProducerService(
	ILogger<ProducerService<KafkaTopicsOutput>> logger,
	IProducerClient producerClient,
	IAdministratorClient administratorClient,
	IOptions<KafkaTopicsOutput> optionsTopics,
	IOptions<KafkaProducerServiceOptions> optionsProducerService,
	IServiceScopeFactory serviceScopeFactory)
	: base(logger, producerClient, administratorClient, optionsTopics, optionsProducerService, serviceScopeFactory) {

	}

	// usa `producerClient` per mandare messaggi a Kafka 
	protected override async Task OperationsAsync(CancellationToken cancellationToken) {
		using IServiceScope scope = ServiceScopeFactory.CreateScope();
		IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();

		IEnumerable<TransactionalOutbox> transactions = await repository.GetAllTransactionalOutboxes(cancellationToken);
		if (!transactions.Any()) {
			Logger.LogInformation("OperationsAsync: no transactions to manage");
			return;
		}

		try {

			foreach (TransactionalOutbox t in transactions) {
				string topic = t.table;

				if (!topic.Equals(KafkaTopicsOutput.Authors))
					throw new Exception($"OperationsAsync: topic <{topic}> is not permitted for this producer.");

				Logger.LogInformation("Message producing...");
				await ProducerClient.ProduceAsync(t.table, t.message, cancellationToken);
				Logger.LogInformation("Message produced... deleting");

				await repository.DeleteTransactionalOutboxFromId(t.id, cancellationToken);
				Logger.LogInformation("DeleteTransactionalOutboxFromId done");

				await repository.SaveChangesAsync(cancellationToken);
				Logger.LogInformation("SaveChangesAsync done");

				string groupMsg = $" record {nameof(TransactionalOutbox)} con " +
					$"{nameof(TransactionalOutbox.id)} = {t.id}, " +
					$"{nameof(TransactionalOutbox.table)} = '{t.table}' e " +
					$"{nameof(TransactionalOutbox.message)} = '{t.message}'";

				Logger.LogInformation("Deleted {groupMsg}...", groupMsg);
			}

		} catch (Exception e) {
			throw e;
		}

		await Task.CompletedTask;
	}
}