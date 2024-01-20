using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Config {

	public abstract class KafkaConfigs {
		public string SectionName = "Kafka";
		public string BootstrapServers = string.Empty;
	}

	// concrete implementations
	public class KafkaAdminClientOptions : KafkaConfigs {
		public KafkaAdminClientOptions() {
			this.SectionName = "Kafka:AdminClient";
		}
	}

	public class KafkaProducerClientOptions : KafkaConfigs {
		public KafkaProducerClientOptions() {
			this.SectionName = "Kafka:ProducerClient";
		}
	}

	public class KafkaConsumerClientOptions : KafkaConfigs {
		public string GroupId;
		public KafkaConsumerClientOptions() {
			this.SectionName = "Kafka:ConsumerClient";
			this.GroupId = string.Empty;
		}
	}

	// another config for a ProducerService
	public class KafkaProducerServiceOptions {
		public string SectionName = "Kafka:ProducerService";
		public int DelaySeconds { get; set; } = 60;
		public int IntervalSeconds { get; set; } = 60;
	}

	public interface IKafkaTopics {
		IEnumerable<string> GetTopics();
	}

	public abstract class AbstractKafkaTopics : IKafkaTopics {
		public string SectionName = "Kafka:Topics";

		public abstract IEnumerable<string> GetTopics();
	}
}