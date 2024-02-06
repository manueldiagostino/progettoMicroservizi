using GlobalUtility.Kafka.Abstraction.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using GlobalUtility.Kafka.Model;
using System.Net;

using GlobalUtility.Kafka.Config;
using System.Text.Json;

namespace GlobalUtility.Kafka.Clients;

/*
	Un Producer si occupa di leggere dalla TransactionalOutbox le ultime modifiche ai DBMS
	e comunica a Kafka i cambiamenti.
*/
public class ProducerClient : IProducerClient {
	private bool _disposed;
	private ILogger<IProducerClient> _logger;
	private IProducer<Null, string> _producer;

	public ProducerClient(IOptions<KafkaProducerClientOptions> options, ILogger<IProducerClient> logger) {
		_disposed = false;
		_logger = logger;
		_producer = new ProducerBuilder<Null, string>(GetProducerconfig(options)).Build();
	}

	private ProducerConfig GetProducerconfig(IOptions<KafkaProducerClientOptions> options) {
		ProducerConfig config = new() {
			BootstrapServers = options.Value.BootstrapServers,
			ClientId = Dns.GetHostName()
		};

		_logger.LogInformation("Kafka ProducerClientConfig: {producerClientConfig}", JsonSerializer.Serialize(config));
		return config;
	}
	public async Task ProduceAsync(string topic, string message, CancellationToken cancellationToken = default) {
		await ProduceAsync(topic, message, null, cancellationToken);
	}
	public async Task ProduceAsync(string topic, int partition, string message, CancellationToken cancellationToken = default) {
		await ProduceAsync(topic, message, partition, cancellationToken);
	}

	private async Task ProduceAsync(string topic, string message, int? partition = null, CancellationToken cancellationToken = default) {
		DeliveryResult<Null, string>? deliveryResult;
        try {

            Message<Null, string> msg = new Message<Null, string> { Value = message };

            if (partition.HasValue) {
                TopicPartition topicPartition = new TopicPartition(topic, partition.Value);
                _logger.LogInformation("Sending message: {msg}; towards the TopicPartition: {topicPartition}", JsonSerializer.Serialize(msg), JsonSerializer.Serialize(topicPartition));
                deliveryResult = await _producer.ProduceAsync(topicPartition, msg, cancellationToken);

            } else {
                _logger.LogInformation("Sending message: {msg}; towards the Topic: '{topic}'", JsonSerializer.Serialize(msg), topic);
                deliveryResult = await _producer.ProduceAsync(topic, msg, cancellationToken);
            }

        } catch (ProduceException<Null, string> ex) {
            _logger.LogError(ex, "ProduceException<Null, string> sollevata all'interno del metodo {methodName}: {reason}", nameof(ProduceAsync), ex.Error.Reason);
            throw;
        } catch (KafkaException ex) {
            _logger.LogError(ex, "KafkaException sollevata all'interno del metodo {methodName}: {reason}", nameof(ProduceAsync), ex.Error.Reason);
            throw;
        } catch (Exception ex) {
            _logger.LogError(ex, "Exception sollevata all'interno del metodo {methodName}: {message}", nameof(ProduceAsync), ex.Message);
            throw;
        }

        _logger.LogInformation("Message send!");
        _logger.LogInformation("deliveryResult: {deliveryResult}", JsonSerializer.Serialize(deliveryResult));
	}
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this); ;
	}

	protected virtual void Dispose(bool disposing) {
		if (!_disposed) {
			if (disposing) {
			}
			_disposed = true;
		}
	}

}
