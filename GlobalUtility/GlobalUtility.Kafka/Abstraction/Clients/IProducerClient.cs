namespace GlobalUtility.Kafka.Abstraction.Clients;

public interface IProducerClient : IDisposable {
	Task ProduceAsync(string topic, int partition, string message, CancellationToken cancellationToken = default);
}
