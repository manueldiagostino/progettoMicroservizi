using System.Text.Json;
using GlobalUtility.Kafka.Exceptions;
using GlobalUtility.Kafka.MessageHandlers;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Business.Kafka;

public class MessageHandler : AbstractOperationMessageHandler<AuthorKafkaDto, IAuthorKafkaRepository> {
	public MessageHandler(ILogger<AbstractOperationMessageHandler<AuthorKafkaDto, IAuthorKafkaRepository>> logger, IAuthorKafkaRepository repository) : base(logger, repository) {

	}

	protected override async Task DeleteAsync(AuthorKafkaDto messageDto, CancellationToken cancellationToken = default) {
		try {
			await Repository.DeleteAuthor(messageDto.AuthorId, cancellationToken);

			Logger.LogInformation($"Delete AuthorKafkaDto: {JsonSerializer.Serialize(messageDto)}");
			await Repository.SaveChangesAsync(cancellationToken);
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while deleting AuthorKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}

	protected override async Task InsertAsync(AuthorKafkaDto messageDto, CancellationToken cancellationToken = default) {
		AuthorKafka authorKafka = new() {
			AuthorId = messageDto.AuthorId,
			Name = messageDto.Name,
			Surname = messageDto.Surname
		};

		try {
			await Repository.InsertAuthor(authorKafka, cancellationToken);

			Logger.LogInformation($"Insert AuthorKafkaDto: {JsonSerializer.Serialize(messageDto)}");
			await Repository.SaveChangesAsync(cancellationToken);
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while inserting AuthorKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}

	protected override async Task UpdateAsync(AuthorKafkaDto messageDto, CancellationToken cancellationToken = default) {
		AuthorKafka authorKafka = new() {
			AuthorId = messageDto.AuthorId,
			Name = messageDto.Name,
			Surname = messageDto.Surname
		};

		try {
			await Repository.UpdateAuthor(authorKafka, cancellationToken);

			Logger.LogInformation($"Update AuthorKafkaDto: {JsonSerializer.Serialize(messageDto)}");
			await Repository.SaveChangesAsync(cancellationToken);
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while updating AuthorKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}
}
