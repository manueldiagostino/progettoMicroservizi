using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Abstraction.MessageHandler;
using GlobalUtility.Kafka.Abstraction.Services;
using GlobalUtility.Kafka.Clients;
using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlobalUtility.Kafka.Services {
	public class ConsumerService<TKafkaTopicsInput> : IConsumerService<TKafkaTopicsInput> where TKafkaTopicsInput : class, IKafkaTopics {
		protected ILogger<ConsumerService<TKafkaTopicsInput>> Logger { get; }
		protected IConsumerClient ConsumerClient { get; }
		protected IAdministratorClient AdministratorClient { get; }
		protected IServiceScopeFactory ServiceScopeFactory { get; }
		protected IMessageHandlerFactory MessageHandlerFactory { get; }
		protected IEnumerable<string> Topics { get; }
		bool _disposedValue;
		public ConsumerService(
		ILogger<ConsumerService<TKafkaTopicsInput>> logger,
		IConsumerClient consumerClient,
		IAdministratorClient adminClient,
		IOptions<TKafkaTopicsInput> optionsTopics,
		IServiceScopeFactory serviceScopeFactory,
		IMessageHandlerFactory messageHandlerFactory) {

			Logger = logger;
			ConsumerClient = consumerClient;
			AdministratorClient = adminClient;
			Topics = optionsTopics.Value.GetTopics();
			ServiceScopeFactory = serviceScopeFactory;
			MessageHandlerFactory = messageHandlerFactory;

			_disposedValue = false;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			Logger.LogInformation("START ConsumerService.ExecuteAsync...");

			foreach (var topic in Topics) {
				while (!AdministratorClient.TopicExists(topic)) {
					await Task.Delay(100, stoppingToken);
				};
			}

			await ConsumerClient.ConsumeInLoopAsync(
				Topics,
				async msg => {
					using IServiceScope scope = ServiceScopeFactory.CreateScope();
					IMessageHandler handler = MessageHandlerFactory.Create(msg.Topic, scope.ServiceProvider); // il tipo giusto viene preso dallo scope
					await handler.OnMessageReceivedAsync(msg.Message.Value);
				},
				stoppingToken
			);

			Logger.LogInformation("STOP ConsumerService");
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposedValue) {
				if (disposing) {
					// Eliminare lo stato gestito (oggetti gestiti)
					ConsumerClient?.Dispose();
					AdministratorClient?.Dispose();
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
}