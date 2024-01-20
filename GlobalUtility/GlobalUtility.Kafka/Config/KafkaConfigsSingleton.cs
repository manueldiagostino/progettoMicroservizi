using Microsoft.Extensions.Options;

namespace GlobalUtility.Kafka.Model;

public class KafkaConfigsSingleton {
	private static readonly object mutex = new();
	private static volatile KafkaConfigsSingleton? instance = null;

	public string BootstrapServers { get; } = string.Empty;

	private KafkaConfigsSingleton(IOptions<KafkaConfigsSingleton> options) {
		BootstrapServers = options.Value.BootstrapServers;
	}

	public static KafkaConfigsSingleton GetInstance(IOptions<KafkaConfigsSingleton> options) {
		if (instance == null) {
			lock (mutex) {
				instance ??= new KafkaConfigsSingleton(options);
			}
		}

		return instance;
	}
}
