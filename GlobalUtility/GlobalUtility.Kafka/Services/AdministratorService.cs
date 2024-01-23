
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Abstraction.Services;
using GlobalUtility.Kafka.Config;
using Confluent.Kafka;

namespace GlobalUtility.Kafka.Services;
public class AdministratorService<TKafkaTopicsInput> : IAdministratorService<TKafkaTopicsInput> where TKafkaTopicsInput : class, IKafkaTopics {
	protected ILogger<AdministratorService<TKafkaTopicsInput>> Logger { get; }
	protected IAdministratorClient AdminClient { get; }
	protected IServiceScopeFactory ServiceScopeFactory { get; }
	protected IEnumerable<string> Topics { get; }

	bool _disposedValue;
	public AdministratorService(
	ILogger<AdministratorService<TKafkaTopicsInput>> logger,
	IAdministratorClient adminClient,
	IOptions<TKafkaTopicsInput> optionsTopics,
	IServiceScopeFactory serviceScopeFactory) {

		Logger = logger;
		AdminClient = adminClient;
		Topics = optionsTopics.Value.GetTopics();
		ServiceScopeFactory = serviceScopeFactory;
		_disposedValue = false;
	}
	private bool IsKafkaAlive() {
		var metadata = AdminClient.GetMetadata(TimeSpan.FromSeconds(5));
		return metadata != null;
	}
	/*
		It handles topics creation on start-up
	*/
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		Logger.LogInformation("START AdministratorService ExecuteAsync");

		// attendo server up
		while (!IsKafkaAlive()) {
			Logger.LogInformation("WAITING Kafka starts");
			Thread.Sleep(1000);
		}

		Logger.LogInformation("WORKING-START AdministratorService is doing work");

		foreach (var topic in Topics) {
			if (AdminClient.TopicExists(topic)) {
				Logger.LogInformation("WORKING-CONTINUE topic <" + topic + "> already exists");

				continue;
			}

			await AdminClient.CreateTopicAsync(topic);
			Logger.LogInformation("WORKING-CREATION AdministratorService created <" + topic + "> topic");

		}

		Logger.LogInformation("WORKING-FINISHED AdministratorService");
		Logger.LogInformation("EXITING AdministratorService ExecuteAsync");
	}

	protected virtual void Dispose(bool disposing) {
		if (!_disposedValue) {
			if (disposing) {
				AdminClient?.Dispose();
				base.Dispose();
			}

			// Liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
			// Impostare campi di grandi dimensioni su Null
			_disposedValue = true;
		}
	}
	public override void Dispose() {
		// Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
