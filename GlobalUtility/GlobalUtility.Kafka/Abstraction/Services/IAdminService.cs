using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.Hosting;

namespace GlobalUtility.Kafka.Abstraction.Services;

public abstract class IAdministratorClientService<TKafkaTopicsInput> : BackgroundService where TKafkaTopicsInput : class, IKafkaTopics {
}
