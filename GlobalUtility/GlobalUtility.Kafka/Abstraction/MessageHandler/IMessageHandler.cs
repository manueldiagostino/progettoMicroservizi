using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Abstraction.MessageHandler {
	public interface IMessageHandler {
		Task OnMessageReceivedAsync(string msg);
	}
}