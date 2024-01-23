
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Abstraction.Services;
using GlobalUtility.Kafka.Config;
using GlobalUtility.Kafka.Abstraction.MessageHandler;
using Confluent.Kafka;
using Microsoft.VisualBasic;

namespace GlobalUtility.Kafka.Services {
	public abstract class ProducerService<TKafkaTopicsOutput> : IProducerService<TKafkaTopicsOutput> where TKafkaTopicsOutput : class, IKafkaTopics {
		protected ILogger<ProducerService<TKafkaTopicsOutput>> Logger { get; }
		protected IProducerClient ProducerClient { get; }
		protected IAdministratorClient AdministratorClient { get; }
		protected TKafkaTopicsOutput KafkaTopics { get; }
		protected IEnumerable<string> Topics { get; }
		protected IServiceScopeFactory ServiceScopeFactory { get; }
		protected int DueTime { get; }
		protected int Period { get; }
		protected CancellationTokenSource StoppingCts { get; } = new CancellationTokenSource();
		protected Task ExecutingTask { get; private set; } = Task.CompletedTask;

		protected Timer? TimerTask { get; private set; }

		bool _disposedValue;

		public ProducerService(
		ILogger<ProducerService<TKafkaTopicsOutput>> logger,
		IProducerClient producerClient,
		IAdministratorClient administratorClient,
		IOptions<TKafkaTopicsOutput> optionsTopics,
		IOptions<KafkaProducerServiceOptions> optionsProducerService,
		IServiceScopeFactory serviceScopeFactory) {

			Logger = logger;
			ProducerClient = producerClient;
			AdministratorClient = administratorClient;
			KafkaTopics = optionsTopics.Value;
			Topics = optionsTopics.Value.GetTopics();
			ServiceScopeFactory = serviceScopeFactory;
			DueTime = optionsProducerService.Value.DelaySeconds;
			Period = optionsProducerService.Value.IntervalSeconds;
			ServiceScopeFactory = serviceScopeFactory;
			_disposedValue = false;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
			Logger.LogInformation("START ProducerService: ExecuteAsync");

			// check AdministratorService
			foreach (var topic in Topics) {
				while (!AdministratorClient.TopicExists(topic)) {
					Logger.LogInformation("WAITING ProducerService: TopicExists");
					await Task.Delay(100, cancellationToken);
				};
			}

			TimerTask = new Timer(DoWork, null, TimeSpan.FromSeconds(DueTime), TimeSpan.FromMilliseconds(Timeout.Infinite));
			Logger.LogInformation("ESC ProducerService: ExecuteAsync");
			await Task.CompletedTask;
		}

		protected void DoWork(object? state) {
			// Blocco il TimerTask per impedire che il metodo ExecuteTask venga invocato nuovamente prima che sia terminata
			// l'esecuzione del metodo ExecuteTaskAsync
			StopTimer();
			ExecutingTask = ExecuteTaskAsync(StoppingCts.Token);
		}

		private async Task ExecuteTaskAsync(CancellationToken cancellationToken) {
			Logger.LogInformation("START ProducerService.ExecuteTaskAsync...");

			try {
				await OperationsAsync(cancellationToken);
			} catch (Exception e) {
				Logger.LogError(e, "Exception sollevata all'interno del metodo {methodName}. Exception Message: {message}",
					nameof(ExecuteTaskAsync), e.Message);
				throw e;
			}

			Logger.LogInformation("STOP ProducerService.ExecuteTaskAsync");

			ActivateTimer();
			await Task.CompletedTask;
		}

		private void ActivateTimer() {
			// Riattivo nuovamente il TimerTask per invocare una sola volta il metodo ExecuteTask dopo che sono trascorsi Period secondi
			TimerTask?.Change(TimeSpan.FromSeconds(Period), TimeSpan.FromMilliseconds(Timeout.Infinite));
		}

		private void StopTimer() {
			// Blocco il TimerTask per impedire che il metodo ExecuteTask venga invocato nuovamente
			TimerTask?.Change(Timeout.Infinite, 0);
		}

		public override async Task StopAsync(CancellationToken cancellationToken) {
			Logger.LogInformation("ProducerService.StopAsync...");

			StopTimer();

			if (ExecutingTask != null && !ExecutingTask.IsCompleted) {
				try {
					// Signal cancellation to the executing method
					StoppingCts.Cancel();
				} finally {
					// Wait until the task completes or the stop token triggers
					await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
				}
			}

			Logger.LogInformation("ProducerService.StopAsync finished");
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposedValue) {
				if (disposing) {
					// Eliminare lo stato gestito (oggetti gestiti)
					ProducerClient?.Dispose();
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