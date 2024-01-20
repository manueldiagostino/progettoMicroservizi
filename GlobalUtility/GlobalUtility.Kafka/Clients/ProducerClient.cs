using GlobalUtility.Kafka.Abstraction.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using GlobalUtility.Kafka.Model;
using System.Net;

using GlobalUtility.Kafka.Config;

namespace GlobalUtility.Kafka.Clients;

/*
	Un Producer si occupa di leggere dalla TransactionalOutbox le ultime modifiche ai DBMS
	e comunica a Kafka i cambiamenti.
*/
public class ProducerClient : IProducerClient {
	private bool _disposed;
	private ILogger<IProducerClient> _logger;
	private IProducer<Null, string> _producer;

	public ProducerClient(IOptions<KafkaConfigs> options, ILogger<IProducerClient> logger) {
		_disposed = false;
		_logger = logger;
		_producer = new ProducerBuilder<Null, string>(GetProducerconfig(options)).Build();
	}

	private ProducerConfig GetProducerconfig(IOptions<KafkaConfigs> options) {
		ProducerConfig res = new ProducerConfig();
		res.BootstrapServers = options.Value.BootstrapServers;
		res.ClientId = Dns.GetHostName();

		return res;
	}
	public Task ProduceAsync(string topic, int partition, string message, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
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
