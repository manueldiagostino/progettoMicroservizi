using AutoMapper;
using GlobalUtility.Kafka.Abstraction.MessageHandler;
using GlobalUtility.Kafka.Abstraction.Messages;
using GlobalUtility.Kafka.Messages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GlobalUtility.Kafka.Constants;
using GlobalUtility.Kafka.Exceptions;

namespace GlobalUtility.Kafka.MessageHandlers;

public abstract class AbstractOperationMessageHandler<TMessageDto, TRepository> : IMessageHandler where TMessageDto : class, new() {

	protected ILogger<AbstractOperationMessageHandler<TMessageDto, TRepository>> Logger { get; }
	protected TRepository Repository { get; }
	protected string MessageDtoType { get; }

	protected AbstractOperationMessageHandler(ILogger<AbstractOperationMessageHandler<TMessageDto, TRepository>> logger, TRepository repository) {
		Logger = logger;
		Repository = repository;
		MessageDtoType = typeof(TMessageDto).Name;
	}

	public Task OnMessageReceivedAsync(string msg, CancellationToken cancellationToken = default) {
		Logger.LogInformation("OperationMessage to handle: '{msg}'...", msg);

		if (string.IsNullOrWhiteSpace(msg)) {
			throw new MessageHandlerException($"Message {nameof(msg)} {nameof(IOperationMessage<TMessageDto>)} cannot be null", nameof(msg));
		}

		return OnMessageReceivedInternalAsync(msg, cancellationToken);

	}
	private async Task OnMessageReceivedInternalAsync(string msg, CancellationToken cancellationToken = default) {
		#region Deserialization and message verification
		Logger.LogInformation("Deserializing OperationMessage with Dto of type {messageDtoType}...", MessageDtoType);

		var options = new JsonSerializerOptions {
			PropertyNameCaseInsensitive = true, // Ignore case sensitivity in property names
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Map property names to CamelCase style
		};

		OperationMessage<TMessageDto>? opMsg;
		try {
			opMsg = JsonSerializer.Deserialize<OperationMessage<TMessageDto>?>(msg, options);
		} catch (Exception ex) {
			throw new MessageHandlerException($"An error occurred while deserializing the message {nameof(msg)} " +
				$"'{msg}' into {nameof(OperationMessage<TMessageDto>)} with {MessageDtoType} as {nameof(OperationMessage<TMessageDto>.Dto)} : {ex}", nameof(msg), ex);
		}

		if (opMsg == null) {
			throw new MessageHandlerException($"The {nameof(opMsg)} {nameof(OperationMessage<TMessageDto>)}, " +
				$"with {MessageDtoType} as {nameof(OperationMessage<TMessageDto>.Dto)}, cannot be null", nameof(msg));
		}

		Logger.LogInformation($"Message deserialization successful: {JsonSerializer.Serialize(opMsg)}");

		opMsg.CheckMessage();
		#endregion

		#region Operation to be executed
		Logger.LogInformation("Executing operation '{operation}'...", opMsg.Operation);
		switch (opMsg.Operation) {
			case Operations.Insert:
				await InsertAsync(opMsg.Dto);
				break;
			case Operations.Update:
				await UpdateAsync(opMsg.Dto);
				break;
			case Operations.Delete:
				await DeleteAsync(opMsg.Dto);
				break;
			default:
				throw new MessageHandlerException($"{nameof(opMsg)}.{nameof(opMsg.Operation)} contains an invalid value '{opMsg.Operation}'", $"{nameof(opMsg)}.{nameof(opMsg.Operation)}");
		}
		Logger.LogInformation("Operation '{operation}' completed successfully", opMsg.Operation);
		#endregion
	}


	protected abstract Task InsertAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);

	protected abstract Task UpdateAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);

	protected abstract Task DeleteAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);
}