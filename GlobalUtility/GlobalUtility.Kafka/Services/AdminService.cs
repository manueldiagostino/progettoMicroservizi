
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Abstraction.Services;
using GlobalUtility.Kafka.Config;
using GlobalUtility.Kafka.Abstraction.MessageHandler;

namespace GlobalUtility.Kafka.Services;
public class AdminService<TKafkaTopicsInput> : IAdministratorClientService<TKafkaTopicsInput> where TKafkaTopicsInput : class, IKafkaTopics {
	protected ILogger<AdminService<TKafkaTopicsInput>> Logger { get; }
	protected IAdministratorClient AdminClient { get; }
	protected IServiceScopeFactory ServiceScopeFactory { get; }
	protected IMessageHandlerFactory MessageHandlerFactory { get; }
	protected IEnumerable<string> Topics { get; }

	bool _disposedValue;
	public AdminService(
	ILogger<AdminService<TKafkaTopicsInput>> logger,
	IAdministratorClient adminClient,
	IOptions<TKafkaTopicsInput> optionsTopics,
	IServiceScopeFactory serviceScopeFactory,
	IMessageHandlerFactory messageHandlerFactory) {

		Logger = logger;
		AdminClient = adminClient;
		Topics = optionsTopics.Value.GetTopics();
		ServiceScopeFactory = serviceScopeFactory;
		MessageHandlerFactory = messageHandlerFactory;
		_disposedValue = false;
	}

	/*
		It handles topics creation on start-up
	*/
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		Logger.LogInformation("START AdminService ExecuteAsync");

		Logger.LogInformation("WORKING-START AdminService is doing work");
		foreach (var topic in Topics) {
			await AdminClient.CreateTopicAsync(topic);
			Logger.LogInformation("WORKING-CREATION AdminService created <" + topic + "> topic");
		}
		Logger.LogInformation("WORKING-FINISHED AdminService is doing work");

		Logger.LogInformation("START AdminService ExecuteAsync");
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
