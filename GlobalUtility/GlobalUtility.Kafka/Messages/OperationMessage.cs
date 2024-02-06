using GlobalUtility.Kafka.Abstraction.Messages;
using GlobalUtility.Kafka.Constants;
using GlobalUtility.Kafka.Exceptions;

namespace GlobalUtility.Kafka.Messages {
	public class OperationMessage<TDto> : IOperationMessage<TDto> where TDto : class, new() {
		public string Operation { get; set; } = string.Empty;
		public TDto Dto { get; set; } = new();

		public void CheckMessage() {

			if (string.IsNullOrWhiteSpace(Operation)) {
				throw new MessageException($"The property {nameof(Operation)} cannot be null or empty", nameof(Operation));
			}

			if (!Operations.IsValid(Operation)) {
				throw new MessageException($"The property {nameof(Operation)} ({typeof(IOperationMessage<TDto>).Name}) contains an invalid value", nameof(Operation));
			}


			if (Dto == null) {
				throw new MessageException($"The property {nameof(Dto)} ({typeof(TDto).Name}) cannot be null", nameof(Dto));
			}
		}

	}


}