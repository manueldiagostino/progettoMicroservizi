using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalUtility.Kafka.Services;
using GlobalUtility.Kafka.Config;
using Microsoft.Extensions.Logging;
using GlobalUtility.Kafka.Abstraction.Clients;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using GlobalUtility.Kafka.Abstraction.MessageHandler;

namespace AuthorsHandler.Business.Kafka {
	public class AdministratorService : AdministratorService<KafkaTopicsOutput> {
		public AdministratorService(
		ILogger<AdministratorService<KafkaTopicsOutput>> logger, 
		IAdministratorClient adminClient, 
		IOptions<KafkaTopicsOutput> optionsTopics, 
		IServiceScopeFactory serviceScopeFactory) 
		: base(logger, adminClient, optionsTopics, serviceScopeFactory) {
	
		}
	}
}