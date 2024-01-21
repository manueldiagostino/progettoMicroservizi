using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlobalUtility.Kafka.Clients {
	public class ConsumerClient : IConsumerClient {
		private IConsumer<Null, string> _consumer;
		private ILogger<ConsumerClient> _logger;
		bool _disposed;

		public ConsumerClient(IOptions<KafkaConsumerClientOptions> options, ILogger<ConsumerClient> logger) {
			_logger = logger;
			_consumer = new ConsumerBuilder<Null, string>(GetConsumerConfig(options)).Build();
		}

		private ConsumerConfig GetConsumerConfig(IOptions<KafkaConsumerClientOptions> options) {
			ConsumerConfig consumerConfig = new ConsumerConfig {
				BootstrapServers = options.Value.BootstrapServers,
				GroupId = options.Value.GroupId,
				ClientId = Dns.GetHostName(),
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false,
				AutoCommitIntervalMs = 0,
				AllowAutoCreateTopics = false
			};

			_logger.LogInformation("Kafka ConsumerConfig: {consumerConfig}", JsonSerializer.Serialize(consumerConfig));

			return consumerConfig;
		}

		public Task<ConsumeResult<Null, string>> ConsumeAsync(CancellationToken cancellationToken) {
			return Task.Run(() => {
				ConsumeResult<Null, string> result;
				try {
					_logger.LogInformation("Poll for new messages...");
					result = _consumer.Consume(cancellationToken);
				} catch (ConsumeException ex) {
					_logger.LogError(ex, "ConsumeException thrown inside method {methodName}: {reason}", nameof(ConsumeAsync), ex.Error.Reason);
					throw;
				} catch (KafkaException ex) {
					_logger.LogError(ex, "KafkaException thrown inside method {methodName}: {reason}", nameof(ConsumeAsync), ex.Error.Reason);
					throw;
				} catch (OperationCanceledException ex) {
					_logger.LogError(ex, "OperationCanceledException thrown inside method {methodName}: {message}", nameof(ConsumeAsync), ex.Message);
					throw;
				} catch (Exception ex) {
					_logger.LogError(ex, "Exception thrown inside method {methodName}: {message}", nameof(ConsumeAsync), ex.Message);
					throw;
				}
				_logger.LogInformation("Consumato il seguente messagio: {result}", JsonSerializer.Serialize(result));
				return result;
			});
		}

		public async Task<bool> ConsumeInLoopAsync(string topic, Func<ConsumeResult<Null, string>, Task> comsumerOperationsAsync, CancellationToken cancellationToken = default) {
			return await ConsumeInLoopAsync(new List<string>() { topic }, comsumerOperationsAsync, cancellationToken);
		}

		public async Task<bool> ConsumeInLoopAsync(IEnumerable<string> topics, Func<ConsumeResult<Null, string>, Task> comsumerOperationsAsync, CancellationToken cancellationToken = default) {
			_logger.LogInformation("START ConsumerClient ConsumeInLoopAsync");

			// Sottoscrizione alla lista di topic
			Subscribe(topics);

			ConsumeResult<Null, string>? result = null;
			// Consume Loop
			while (!cancellationToken.IsCancellationRequested) {

				try {
					// Reading the message
					result = await ConsumeAsync(cancellationToken);

					try {
						// Processing the message
						await comsumerOperationsAsync(result);
					} catch (Exception ex) {
						_logger.LogWarning(ex, "Exception thrown inside Func {funcName}, for the following ConsumeResult: {result}. Exception Message: {message}",
							nameof(comsumerOperationsAsync), JsonSerializer.Serialize(result), ex.Message);
						throw;
					}

					_logger.LogInformation("Func {funcName} completed!", nameof(comsumerOperationsAsync));
				} catch (Exception ex) {
					_logger.LogError(ex, "Exception thrown inside method {methodName}. Exception Message: {message}",
						nameof(ConsumeInLoopAsync), ex.Message);
				}


				Commit(result);

			}

			_logger.LogInformation("END ConsumerClient ConsumeInLoopAsync");

			return true;
		}


		public List<string> GetCurrentSubscription() {
			throw new NotImplementedException();
		}

		public void Subscribe(IEnumerable<string> topics) {
			try {
				_logger.LogInformation("Sottoscrizione ai seguenti topic: '{topics}'...", string.Join("', '", topics));
				_consumer.Subscribe(topics);
			} catch (Exception ex) {
				_logger.LogError(ex, "Exception sollevata all'interno del metodo {methodName}. Exception Message: {message}", nameof(Subscribe), ex.Message);
				throw;
			}
			_logger.LogInformation("Sottoscrizione completata!");
		}

		public void Subscribe(string topic) {
			Subscribe(new List<string>() { topic });
		}

		public void Unsubscribe() {
			try {
				_logger.LogInformation("Cancelling current subscriptions...");
				_consumer.Unsubscribe();
			} catch (Exception ex) {
				_logger.LogWarning(ex, "Exception thrown inside method {methodName}. Exception Message: {message}", nameof(Unsubscribe), ex.Message);
				throw;
			}
			_logger.LogInformation("Cancellation of current subscriptions completed");
		}

		public void Commit(ConsumeResult<Null, string>? result) {
			try {
				if (result != null) {
					_logger.LogDebug("Commit offset: {result}", JsonSerializer.Serialize(result));
					_consumer.Commit(result);
					//_consumer.StoreOffset(result);
				}
			} catch (TopicPartitionOffsetException ex) {
				_logger.LogCritical(ex, "TopicPartitionOffsetException thrown inside method {methodName}: {reason}", nameof(Commit), ex.Error.Reason);
				throw;
			} catch (KafkaException ex) {
				_logger.LogCritical(ex, "KafkaException thrown inside method {methodName}: {reason}", nameof(Commit), ex.Error.Reason);
				throw;
			} catch (Exception ex) {
				_logger.LogCritical(ex, "Exception thrown inside method {methodName}: {message}", nameof(Commit), ex.Message);
				throw;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this); ;
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					// Rilascia risorse gestite
					// Chiudi file, connessioni di rete, etc.
				}

				// Rilascia risorse non gestite
				// Libera handle di sistema, etc.

				_disposed = true;
			}
		}
	}
}