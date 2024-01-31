using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlobalUtility.Kafka.Clients {
	public class AdministratorClient : IAdministratorClient {
		private IAdminClient _adminClient;
		private ILogger<AdministratorClient> _logger;
		private bool _disposed;

		public AdministratorClient(IOptions<KafkaAdminClientOptions> options, ILogger<AdministratorClient> logger) {
			_adminClient = new AdminClientBuilder(GetAdminClientConfig(options)).Build();
			_logger = logger;
			_disposed = false;

			if (_logger == null)
				throw new ArgumentException("_logger is null");
		}

		private AdminClientConfig GetAdminClientConfig(IOptions<KafkaAdminClientOptions> options) {
			AdminClientConfig config = new() {
				BootstrapServers = options.Value.BootstrapServers,
				ClientId = Dns.GetHostName()
			};

			Console.WriteLine("config[BootstrapServers]: " + config.BootstrapServers);
			Console.WriteLine("config[ClientId]: " + config.ClientId);
			return config;
		}

		public async Task CreatePartitionsAsync(string topic, int increaseTo = 1, CreatePartitionsOptions? options = null) {
			_logger.LogInformation("Cretion of {increaseTo} partitions in topic <{topic}>", increaseTo, topic);

			await TryCatchAsync(
				() =>
					_adminClient.CreatePartitionsAsync(
						new PartitionsSpecification[] {
							new PartitionsSpecification { Topic = topic, ReplicaAssignments = null, IncreaseTo = increaseTo }
						},
						options
					),
					nameof(CreatePartitionsAsync));

			_logger.LogInformation("Cretion of {increaseTo} partitions in topic <{topic}> completed!", increaseTo, topic);
		}

		public async Task CreateTopicAsync(string topic, short replicationFactor = 1, int numPartitions = 1) {
			_logger.LogInformation("Creation of topic <{topic}>", topic);

			await TryCatchAsync(
				() =>
					_adminClient.CreateTopicsAsync(
						new TopicSpecification[] {
							new TopicSpecification { Name = topic, ReplicationFactor = replicationFactor, NumPartitions = numPartitions}
						}
					),
					nameof(CreateTopicAsync)
			);

			_logger.LogInformation("Creation of <{topic}> completed!", topic);
		}

		public async Task CreateTopicsAsync(IEnumerable<string> topics, short replicationFactor = 1, int numPartitions = 1) {
			_logger.LogInformation("Creation of topics <{topic}>", topics);

			foreach (var topic in topics) {
				await CreateTopicAsync(topic, replicationFactor, numPartitions);
			}

			_logger.LogInformation("Creation of topics <{topic}> completed", topics);
		}

		private async Task TryCatchAsync(Func<Task> func, string methodName) {
			try {
				await func();
			} catch (CreateTopicsException ex) {
				_logger.LogError(ex, "CreateTopicsException sollevata all'interno del metodo {methodName}: {reason}", methodName, ex.Error.Reason);
				throw;
			} catch (CreatePartitionsException ex) {
				_logger.LogError(ex, "CreatePartitionsException sollevata all'interno del metodo {methodName}: {reason}", methodName, ex.Error.Reason);
				throw;
			} catch (DeleteTopicsException ex) {
				_logger.LogError(ex, "DeleteTopicsException sollevata all'interno del metodo {methodName}: {reason}", methodName, ex.Error.Reason);
				throw;
			} catch (KafkaException ex) {
				_logger.LogError(ex, "KafkaException sollevata all'interno del metodo {methodName}: {reason}", methodName, ex.Error.Reason);
				throw;
			} catch (Exception ex) {
				_logger.LogError(ex, "Exception sollevata all'interno del metodo {methodName}: {message}", methodName, ex.Message);
				throw;
			}
		}
		public async Task DeleteTopicsAsync(IEnumerable<string> topics, DeleteTopicsOptions? options = null) {
			_logger.LogInformation("Cancellazione dei topic '{topics}'...", string.Join("', '", topics));
			await TryCatchAsync(() => _adminClient.DeleteTopicsAsync(topics, options), nameof(DeleteTopicsAsync));
			_logger.LogInformation("Cancellazione dei topic '{topics}' completata!", string.Join("', '", topics));
		}
		public Metadata? GetMetadata(string? topic = null) {

			TimeSpan timeSpan = TimeSpan.FromSeconds(30);
			string topicInfo = string.Empty;

			Metadata? meta = null;
			try {
				if (string.IsNullOrWhiteSpace(topic)) {
					meta = _adminClient.GetMetadata(timeSpan);
				} else {
					topicInfo = $"per il Topic '{topic}'";
					meta = _adminClient.GetMetadata(topic, timeSpan);
				}

				_logger.LogInformation("Kafka Cluster Metadata {topicInfo}: {meta}", topicInfo, meta.ToString());
			} catch (KafkaException ex) {
				Console.WriteLine($"Errore durante l'ottenimento dei metadati del cluster Kafka: {ex.Message}");
			}

			return meta;
		}

		public Metadata? GetMetadata(TimeSpan timeSpan) {
			try {
				return _adminClient.GetMetadata(timeSpan);
			} catch (KafkaException) {
				return null;
			}
		}

		public bool TopicExists(string topic) {
			Metadata? res = GetMetadata(topic);
			if (res == null)
				return false;

			var topics = res.Topics.Select(t => t.Topic).ToList();
		
			return topics.Contains(topic);
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