using Microsoft.Extensions.Hosting;
using GlobalUtility.Kafka.Config;
using Confluent.Kafka.Admin;
using Confluent.Kafka;

namespace GlobalUtility.Kafka.Abstraction.Clients {
	public interface IAdministratorClient : IDisposable {
		public Task CreateTopicAsync(string topic, short replicationFactor = 1, int numPartitions = 1);
		public Task CreateTopicsAsync(IEnumerable<string> topics, short replicationFactor = 1, int numPartitions = 1);
		public Task DeleteTopicsAsync(IEnumerable<string> topics, DeleteTopicsOptions? options = null);
		public Task CreatePartitionsAsync(string topic, int increaseTo = 1, CreatePartitionsOptions? options = null);

		public Metadata GetMetadata(string? topic = null);
		public bool TopicExists(string topic); 
	}
}