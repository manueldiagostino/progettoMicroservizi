using Microsoft.Extensions.Hosting;
using GlobalUtility.Kafka.Config;

namespace GlobalUtility.Kafka.Abstraction.Clients {
	public interface IAdminClient : IDisposable {
		public Task CreateTopicAsync(string topic, short replicationFactor = 1, int numPartitions = 1);
		public Task DeleteTopicAsync(string topic, short replicationFactor = 1, int numPartitions = 1);
		public Task CreatePartitionAsync(string topic, int increaseTo = 1);
	}
}