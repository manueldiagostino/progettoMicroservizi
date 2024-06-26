using GlobalUtility.Kafka.Abstraction.MessageHandler;
using GlobalUtility.Kafka.Abstraction.Clients;
using GlobalUtility.Kafka.Config;
using GlobalUtility.Kafka.Clients;
using GlobalUtility.Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class KafkaExtensions {

	public static IServiceCollection AddKafkaServices<TKafkaTopicsInput, TKafkaTopicsOutput, TMessageHandlerFactory, TProducerService>(
		this IServiceCollection services, IConfiguration configuration)
		where TKafkaTopicsInput : class, IKafkaTopics
		where TKafkaTopicsOutput : class, IKafkaTopics
		where TMessageHandlerFactory : class, IMessageHandlerFactory
		where TProducerService : ProducerService<TKafkaTopicsOutput> {

		services.AddAdministatorClient(configuration);

		services.AddAdministratorService<TKafkaTopicsOutput>(configuration);

		services.AddConsumerService<TKafkaTopicsInput, TMessageHandlerFactory>(configuration);

		services.AddProducerService<TKafkaTopicsOutput, TProducerService>(configuration);

		return services;
	}

	public static IServiceCollection AddKafkaConsumerService<TKafkaTopicsInput, TMessageHandlerFactory>(
		this IServiceCollection services, IConfiguration configuration)
		where TKafkaTopicsInput : class, IKafkaTopics
		where TMessageHandlerFactory : class, IMessageHandlerFactory {

		services.AddAdministatorClient(configuration);

		services.AddConsumerService<TKafkaTopicsInput, TMessageHandlerFactory>(configuration);

		return services;
	}
	
	public static IServiceCollection AddKafkaProducerService<TKafkaTopicsOutput, TProducerService>(
		this IServiceCollection services, IConfiguration configuration)
		where TKafkaTopicsOutput : class, IKafkaTopics
		where TProducerService : ProducerService<TKafkaTopicsOutput> {

		services.AddAdministatorClient(configuration);

		services.AddProducerService<TKafkaTopicsOutput, TProducerService>(configuration);

		return services;
	}

	public static IServiceCollection AddKafkaAdministratorService<TKafkaTopicsOutput> (
		this IServiceCollection services, IConfiguration configuration)
		where TKafkaTopicsOutput : class, IKafkaTopics {
		
		services.AddAdministatorClient(configuration);
		services.AddAdministratorService<TKafkaTopicsOutput>(configuration);

		return services;
	}

	private static IServiceCollection AddAdministatorClient(this IServiceCollection services, IConfiguration configuration) {
		// KafkaAdminClientOptions
		services.Configure<KafkaAdminClientOptions>(
			configuration.GetSection(KafkaAdminClientOptions.SectionName));
		// AdministatorClient
		services.AddSingleton<IAdministratorClient, AdministratorClient>();

		return services;
	}

	private static IServiceCollection AddAdministratorService<TKafkaTopicsOutput>(
	this IServiceCollection services, IConfiguration configuration)
	where TKafkaTopicsOutput : class, IKafkaTopics {

		// Background AdministratorService
		services.AddSingleton<IHostedService, AdministratorService<TKafkaTopicsOutput>>();

		// KafkaTopicsInput
		services.Configure<TKafkaTopicsOutput>(
			configuration.GetSection(AbstractKafkaTopics.SectionName));

		return services;
	}

	private static IServiceCollection AddConsumerService<TKafkaTopicsInput, TMessageHandlerFactory>(
	this IServiceCollection services, IConfiguration configuration)
	where TKafkaTopicsInput : class, IKafkaTopics
	where TMessageHandlerFactory : class, IMessageHandlerFactory {

		// Background ConsumerService
		services.AddSingleton<IHostedService, ConsumerService<TKafkaTopicsInput>>();

		// MessageHandlerFactory
		services.AddSingleton<IMessageHandlerFactory, TMessageHandlerFactory>();

		// KafkaConsumerClientOptions
		services.Configure<KafkaConsumerClientOptions>(configuration.GetSection(KafkaConsumerClientOptions.SectionName));

		// ConsumerClient
		services.AddSingleton<IConsumerClient, ConsumerClient>();

		// KafkaTopicsInput
		services.Configure<TKafkaTopicsInput>(
			configuration.GetSection(AbstractKafkaTopics.SectionName));

		return services;
	}

	private static IServiceCollection AddProducerService<TKafkaTopicsOutput, TProducerService>(
	this IServiceCollection services, IConfiguration configuration)
	where TKafkaTopicsOutput : class, IKafkaTopics
	where TProducerService : ProducerService<TKafkaTopicsOutput> {

		// KafkaProducerServiceOptions
		services.Configure<KafkaProducerServiceOptions>(
			configuration.GetSection(KafkaProducerServiceOptions.SectionName));

		// Background ProducerService
		services.AddSingleton<IHostedService, TProducerService>();

		// KafkaProducerClientOptions
		services.Configure<KafkaProducerClientOptions>(
			configuration.GetSection(KafkaProducerClientOptions.SectionName));
		// ProducerClient
		services.AddSingleton<IProducerClient, ProducerClient>();

		// KafkaTopicsOutput
		services.Configure<TKafkaTopicsOutput>(
			configuration.GetSection(AbstractKafkaTopics.SectionName));

		return services;
	}
}