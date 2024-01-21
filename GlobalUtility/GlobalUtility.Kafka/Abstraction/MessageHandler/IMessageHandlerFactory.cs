using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Abstraction.MessageHandler {
	public interface IMessageHandlerFactory {
		IMessageHandler Create(string topic, IServiceProvider serviceProvider);
	}
}