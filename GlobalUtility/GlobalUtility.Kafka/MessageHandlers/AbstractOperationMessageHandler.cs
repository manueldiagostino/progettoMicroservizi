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
		Logger.LogInformation("Messaggio OperationMessage da elaborare: '{msg}'...", msg);

		if (string.IsNullOrWhiteSpace(msg)) {
			throw new MessageHandlerException($"Il messaggio {nameof(msg)} {nameof(IOperationMessage<TMessageDto>)} non può essere null", nameof(msg));
		}

		return OnMessageReceivedInternalAsync(msg, cancellationToken);

	}

	private async Task OnMessageReceivedInternalAsync(string msg, CancellationToken cancellationToken = default) {
		#region Deserializzazione e verifica del messaggio
		Logger.LogInformation("Deserializzazione del messaggio OperationMessage con Dto di tipo {messageDtoType}...", MessageDtoType);

		var options = new JsonSerializerOptions{
    		PropertyNameCaseInsensitive = true, // Ignora la distinzione tra maiuscole e minuscole nei nomi delle proprietà
    		PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Mappa i nomi delle proprietà in stile CamelCase
		};

		OperationMessage<TMessageDto>? opMsg;
		try {
			opMsg = JsonSerializer.Deserialize<OperationMessage<TMessageDto>?>(msg, options);
		} catch (Exception ex) {
			throw new MessageHandlerException($"Si è verificato un errore durante la deserializzazione del messaggio {nameof(msg)} " +
				$"'{msg}' in {nameof(OperationMessage<TMessageDto>)} con {MessageDtoType} come {nameof(OperationMessage<TMessageDto>.Dto)} : {ex}", nameof(msg), ex);
		}

		if (opMsg == null) {
			throw new MessageHandlerException($"Il {nameof(opMsg)} {nameof(OperationMessage<TMessageDto>)}, " +
				$"con {MessageDtoType} come {nameof(OperationMessage<TMessageDto>.Dto)}, non può essere null", nameof(msg));
		}

		Logger.LogInformation($"Deserializzazione del messaggio eseguita correttamente: {JsonSerializer.Serialize(opMsg)}");

		opMsg.CheckMessage();
		#endregion

		#region Operazione da eseguire
		Logger.LogInformation("Esecuzione operazione '{operation}'...", opMsg.Operation);
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
				throw new MessageHandlerException($"{nameof(opMsg)}.{nameof(opMsg.Operation)} contiene un valore non valido '{opMsg.Operation}'", $"{nameof(opMsg)}.{nameof(opMsg.Operation)}");
		}
		Logger.LogInformation("Operazione '{operation}' completata con successo", opMsg.Operation);
		#endregion

	}

	protected abstract Task InsertAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);

	protected abstract Task UpdateAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);

	protected abstract Task DeleteAsync(TMessageDto messageDto, CancellationToken cancellationToken = default);
}