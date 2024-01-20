namespace GlobalUtility.Kafka.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Abstraction.Services;
using GlobalUtility.Kafka.Config;
public class AdminService<TKafkaTopicsInput> : IAdminService<TKafkaTopicsInput> where TKafkaTopicsInput : class, IKafkaTopics {
	protected ILogger<AdminService<TKafkaTopicsInput>> Logger { get; }
	protected IAdminClient AdminClient { get; }
	protected IServiceScopeFactory ServiceScopeFactory { get; }
	// protected IMessageHandlerFactory MessageHandlerFactory { get; }
	protected IEnumerable<string> Topics { get; }

	bool _disposedValue;
	public AdminService(ILogger<AdminService<TKafkaTopicsInput>> logger,
		IAdminClient consumerClient,
		IAdminClient adminClient,
		IOptions<TKafkaTopicsInput> optionsTopics,
		IServiceScopeFactory serviceScopeFactory/*, 
		IMessageHandlerFactory messageHandlerFactory*/
		) {

		Logger = logger;
		AdminClient = adminClient;
		Topics = optionsTopics.Value.GetTopics();
		ServiceScopeFactory = serviceScopeFactory;
		// MessageHandlerFactory = messageHandlerFactory;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		foreach (var topic in Topics) {
			await AdminClient.CreateTopicAsync(topic);
		}
	}
}
