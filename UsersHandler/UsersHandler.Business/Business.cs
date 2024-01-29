using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using UsersHandler.Business.Abstraction;
using UsersHandler.Repository.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;
using GlobalUtility.Manager.Exceptions;
using GlobalUtility.Manager.Operations;
using Newtonsoft.Json;
using GlobalUtility.Kafka.Factory;
using UsersHandler.Business.Kafka;

namespace UsersHandler.Business;

public class Business : IBusiness {
	private readonly IRepository _repository;
	private readonly ILogger _logger;

	public Business(IRepository repository, ILogger<Business> logger) {
		_repository = repository;
		_logger = logger;
	}

	public async Task<int> CreateUser(UserDto userDto, CancellationToken cancellationToken = default) {
		if (userDto == null)
			throw new BusinessException("userDto == null", nameof(userDto));

		await _repository.CreateUser(userDto, cancellationToken = default);
		int changes = await _repository.SaveChangesAsync(cancellationToken);

		if (changes < 0)
			throw new BusinessException("changes < 0");

		var newUser = new UserTransactionalDto() {
			UserId = await _repository.GetIdFromUsername(userDto.Username, cancellationToken),
			Username = userDto.Username,
			Name = userDto.Name,
			Surname = userDto.Surname
		};

		await _repository.InsertTransactionalOutbox(TransactionalOutboxFactory.CreateInsert<UserTransactionalDto>(newUser, KafkaTopicsOutput.Users), cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);
		_logger.LogInformation($"Added <{changes}> users for userDto <{JsonConvert.SerializeObject(userDto)}>");
		return changes;
	}

	public async Task<User> GetUserFromId(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		return await _repository.GetUserFromId(userId, cancellationToken);
	}

	public async Task<User> GetUserFromUsername(string username, CancellationToken cancellationToken = default) {
		if (string.IsNullOrWhiteSpace(username))
			throw new BusinessException("string.IsNullOrWhiteSpace(username)", nameof(username));

		return await _repository.GetUserFromUsername(username, cancellationToken);
	}

	public async Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default) {
		if (string.IsNullOrWhiteSpace(username))
			throw new BusinessException("string.IsNullOrWhiteSpace(username)", nameof(username));

		return await _repository.GetIdFromUsername(username, cancellationToken);
	}

	public async Task<User> UpdateUsername(int userId, string username, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));
		if (string.IsNullOrWhiteSpace(username))
			throw new BusinessException("string.IsNullOrWhiteSpace(username)", nameof(username));

		User user = await _repository.UpdateUsername(userId, username, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);

		_logger.LogInformation($"Updated user <{user.Id},{user.Username}> with username <{username}> ");
		return user;
	}

	public async Task<User> DeleteUser(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		User user = await _repository.DeleteUser(userId, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);

		_logger.LogInformation($"User <{userId}, {user.Username}> deleted");
		return user;
	}


	public async Task<string> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		string? relativePath = await _repository.GetProfilePictureFromId(userId, cancellationToken);

		if (relativePath == null)
			throw new BusinessException("GetProfilePictureFromId: relativePath == null");

		return Files.GetAbsolutePath(relativePath);
	}

	public async Task<string> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default) {
		int id = await GetIdFromUsername(username, cancellationToken);
		return await GetProfilePictureFromId(id, cancellationToken);
	}

	public async Task<User> UploadProfilePictureFromId(int userId, IFormFile profilePicture, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));
		if (profilePicture == null)
			throw new BusinessException("profilePicture == null", nameof(profilePicture));

		string? relativePath = Files.SaveFileToDir(Path.Combine("ProfilePictures"), profilePicture);

		if (relativePath == null)
			throw new BusinessException("SaveFileToDisk returned path == null");

		var res = await _repository.UploadProfilePictureFromId(userId, relativePath, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);

		_logger.LogInformation($"Profile picture for id <{userId}> saved to <{Files.GetAbsolutePath(relativePath)}>");
		return res;
	}

	public async Task<User> DeleteImage(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		User user = await _repository.DeleteImage(userId, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);
		_logger.LogInformation($"Profile picture for id <{userId}> deleted");

		return user;
	}

	public async Task<User> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default) {
		if (bioDto == null)
			throw new BusinessException("bioDto == null", nameof(BioDto));

		User user = await _repository.CreateBioFromId(bioDto, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);
		_logger.LogInformation($"Created bio for user <{user.Id},{user.Username}>");

		return user;
	}

	public async Task<User> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default) {
		if (bioDto == null)
			throw new BusinessException("bioDto == null", nameof(BioDto));

		User user = await _repository.SetBioFromId(bioDto, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);
		_logger.LogInformation($"Set bio for user <{user.Id},{user.Username}>");

		return user;
	}

	public async Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		string? bio = await _repository.GetBioFromId(userId, cancellationToken);
		_logger.LogInformation($"Got bio for user id <{userId}>");

		return bio;
	}

	public async Task<User> DeleteBioFromId(int userId, CancellationToken cancellationToken = default) {
		if (userId <= 0)
			throw new BusinessException("userId <= 0", nameof(userId));

		User user = await _repository.DeleteBioFromId(userId, cancellationToken);
		await _repository.SaveChangesAsync(cancellationToken);
		_logger.LogInformation($"Deleted bio for user id <{user.Id},{user.Username}>");

		return user;
	}
}
