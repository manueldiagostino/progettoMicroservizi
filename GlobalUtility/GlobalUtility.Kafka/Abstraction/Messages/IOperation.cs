namespace GlobalUtility.Kafka.Abstraction.Messages {
	public interface IOperation<TDto> where TDto : class, new() {
		public string Name { get; }

		/* 
			The concrete implementation of an IOperation<TDto> defines
			the Execute method in the proper way; for example, an Insert
			IOperation will have a reference to a Repository and it can
			perform the insertion of the dto correctly.
		*/
		public Task Execute(TDto dto, CancellationToken cancellationToken = default);
	}
	public class DefaultOperation<TDto> : IOperation<TDto> where TDto : class, new() {
		public string Name { get => "Default"; }

		public Task Execute(TDto dto, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}
	}
}