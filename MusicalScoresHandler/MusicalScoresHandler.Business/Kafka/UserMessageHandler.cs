namespace MusicalScoresHandler.Business.Kafka;

using System.Text.Json;
using GlobalUtility.Kafka.Exceptions;
using GlobalUtility.Kafka.MessageHandlers;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

public class UserMessageHandler : AbstractOperationMessageHandler<UserKafkaDto, IUserKafkaRepository> {
	public UserMessageHandler(ILogger<AbstractOperationMessageHandler<UserKafkaDto, IUserKafkaRepository>> logger, IUserKafkaRepository repository) : base(logger, repository) {

	}

	protected override async Task DeleteAsync(UserKafkaDto messageDto, CancellationToken cancellationToken = default) {
		try {
			await Repository.DeleteUser(messageDto.UserId, cancellationToken);

			await Repository.SaveChangesAsync(cancellationToken);
			Logger.LogInformation($"Delete UserKafkaDto: {JsonSerializer.Serialize(messageDto)}");
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while deleting UserKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}

	protected override async Task InsertAsync(UserKafkaDto messageDto, CancellationToken cancellationToken = default) {
		UserKafka userKafka = new() {
			UserId = messageDto.UserId,
			Username = messageDto.Username,
			Name = messageDto.Name,
			Surname = messageDto.Surname
		};

		try {
			await Repository.InsertUser(userKafka, cancellationToken);

			await Repository.SaveChangesAsync(cancellationToken);
			Logger.LogInformation($"Insert UserKafkaDto: {JsonSerializer.Serialize(messageDto)}");
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while inserting UserKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}

	protected override async Task UpdateAsync(UserKafkaDto messageDto, CancellationToken cancellationToken = default) {
		UserKafka userKafka = new() {
			UserId = messageDto.UserId,
			Username = messageDto.Username,
			Name = messageDto.Name,
			Surname = messageDto.Surname
		};

		try {
			await Repository.UpdateUser(userKafka, cancellationToken);

			await Repository.SaveChangesAsync(cancellationToken);
			Logger.LogInformation($"Update UserKafkaDto: {JsonSerializer.Serialize(messageDto)}");
		} catch (Exception e) {
			throw new MessageHandlerException($"Error while updating UserKafkaDto {JsonSerializer.Serialize(messageDto)}", e);
		}
	}
}
