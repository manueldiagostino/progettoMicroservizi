using Microsoft.Extensions.DependencyInjection;
using GlobalUtility.Kafka.Config;

namespace UsersHandler.Business.Kafka;

public class KafkaTopicsOutput : AbstractKafkaTopics {
    public static string Users { get; set; } = "Users";

    public override IEnumerable<string> GetTopics() => new List<string>() { Users };

}