using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Abstraction.Messages {
	public interface IOperationMessage<TDto> where TDto : class, new() {
		public IOperation<TDto> Operation { get; set; }
		public TDto Dto { get; set; }
		public void CheckMessage();
	}
}