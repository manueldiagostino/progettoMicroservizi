using Microsoft.Extensions.DependencyInjection;
using GlobalUtility.Kafka.Config;

namespace AuthorsHandler.Business.Kafka;

public class KafkaTopicsOutput : AbstractKafkaTopics {
    public static string Authors { get; set; } = "Authors";

    public override IEnumerable<string> GetTopics() => new List<string>() { Authors };

}