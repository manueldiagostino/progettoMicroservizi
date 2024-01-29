using System.Text.Json;
using GlobalUtility.Kafka.Constants;
using GlobalUtility.Kafka.Messages;
using GlobalUtility.Kafka.Model;

namespace GlobalUtility.Kafka.Factory {
	public static class TransactionalOutboxFactory {
        public static TransactionalOutbox CreateInsert<TDto>(TDto dto, string tableName) where TDto : class, new() {
            return Create<TDto>(dto, tableName, Operations.Insert);
        }

		public static TransactionalOutbox CreateUpdate<TDto>(TDto dto, string tableName) where TDto : class, new() {
            return Create<TDto>(dto, tableName, Operations.Update);
        }

		public static TransactionalOutbox CreateDelete<TDto>(TDto dto, string tableName) where TDto : class, new() {
            return Create<TDto>(dto, tableName, Operations.Delete);
        }

        private static TransactionalOutbox Create<TDto>(TDto dto, string tableName, string operation) where TDto : class, new() {
            OperationMessage<TDto> opMsg = new OperationMessage<TDto>() {
                Dto  = dto,
                Operation = operation
            };
            opMsg.CheckMessage();

            return new TransactionalOutbox() {
                table = tableName,
                message = JsonSerializer.Serialize(opMsg)
            };
        }
    }
}