using GlobalUtility.Kafka.Abstraction.MessageHandler;
using GlobalUtility.Kafka.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MusicalScoresHandler.Business.Kafka;

public class MessageHandlerFactory : IMessageHandlerFactory {
	private KafkaTopicsInput _options;
	public MessageHandlerFactory(IOptions<KafkaTopicsInput> options) {
		_options = options.Value;
	}

	public IMessageHandler Create(string topic, IServiceProvider serviceProvider) {
		if (topic.Equals(_options.Authors))
			return ActivatorUtilities.CreateInstance<AuthorMessageHandler>(serviceProvider);
		else if (topic.Equals(_options.Users))
			return ActivatorUtilities.CreateInstance<UserMessageHandler>(serviceProvider);

		throw new MessageHandlerException($"Topic <{topic}> not allowed");
	}
}
