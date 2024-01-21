using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Config {

	public abstract class KafkaConfigs {
		public static string SectionName { get; set; } = "Kafka";
		public string BootstrapServers { get; set; } = string.Empty;
	}

	// concrete implementations
	public class KafkaAdminClientOptions : KafkaConfigs {
		public new static string SectionName { get; set; } = "Kafka:AdminClient";
	}

	public class KafkaProducerClientOptions : KafkaConfigs {
		public new static string SectionName { get; set; } = "Kafka:ProducerClient";
		public int DelaySeconds { get; set; } = 60;

		public int IntervalSeconds { get; set; } = 60;
	}

	public class KafkaConsumerClientOptions : KafkaConfigs {
		public new static string SectionName { get; set; } = "Kafka:ConsumerClient";
		public string GroupId { get; set; } = string.Empty;
		public KafkaConsumerClientOptions() {
			GroupId = string.Empty;
		}
	}

	// another config for a ProducerService
	public class KafkaProducerServiceOptions {
		public static string SectionName = "Kafka:ProducerService";
		public int DelaySeconds { get; set; } = 60;
		public int IntervalSeconds { get; set; } = 60;
	}

	public interface IKafkaTopics {
		IEnumerable<string> GetTopics();
	}

	public abstract class AbstractKafkaTopics : IKafkaTopics {
		public static string SectionName { get; set; } = "Kafka:Topics";

		public abstract IEnumerable<string> GetTopics();
	}
}