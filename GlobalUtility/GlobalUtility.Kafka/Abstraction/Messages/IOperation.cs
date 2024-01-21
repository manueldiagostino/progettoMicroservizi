using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalUtility.Kafka.Abstraction.Messages {
	public interface IOperation<TDto> where TDto : class, new() {
		public string Name { init; get; }

		/* 
			The concrete implementation of an IOperation<TDto> defines
			the Execute method in the proper way; for example, an Insert
			IOperation will have a reference to a Repository and it can
			perform the insertion of the dto correctly.
		*/
		public void Execute(TDto dto); 
	}
}