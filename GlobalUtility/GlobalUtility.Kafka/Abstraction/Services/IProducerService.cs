using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.Hosting;

namespace GlobalUtility.Kafka.Abstraction.Services {
	public abstract class IProducerService<TKafkaTopicsOutput> : BackgroundService where TKafkaTopicsOutput : class, IKafkaTopics {
		protected abstract Task OperationsAsync(CancellationToken cancellationToken);
	}
}