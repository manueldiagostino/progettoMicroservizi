using SixLabors.ImageSharp;
using UsersHandler.Repository.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;

using Microsoft.EntityFrameworkCore;
using GlobalUtility.Manager.Exceptions;
using GlobalUtility.Manager.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UsersHandler.Repository;

public class Repository : IRepository {
	protected UsersHandlerDbContext _dbContext;
	private readonly ILogger _logger;


	public Repository(UsersHandlerDbContext usersHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = usersHandlerDbContext;
		_logger = logger;
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		return await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task CreateUser(UserDto userDto, CancellationToken cancellationToken = default) {
		var (salt, hash) = PasswordHasher.HashPassword(userDto.Password);

		User target = new User() {
			Username = userDto.Username,
			Name = userDto.Name,
			Surname = userDto.Surname,
			PropicPath = null,
			Timestamp = PasswordHasher.GetCurrentTimestampInSeconds(),
			BioId = null,
			Salt = salt,
			Hash = hash
		};

		await _dbContext.AddAsync(target, cancellationToken);
	}

	public async Task<User> UploadProfilePictureFromId(int userId, string picturePath, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);

		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for userId <{userId}>");
		
		User user = userList[0];

		string? filePath = user.PropicPath;
		bool deleted = Files.DeleteFileIfExists(filePath);

		if (deleted)
			_logger.LogInformation($"Image {filePath} deleted from disk");

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.PropicPath, picturePath)
		, cancellationToken);

		return user;
	}

	public async Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Username.Equals(username));

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);

		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for username <{username}>");

		return userList[0].Id;
	}

	public async Task<User> GetUserFromId(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);

		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		return userList[0];
	}

	public async Task<User> GetUserFromUsername(string username, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Username.Equals(username));

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);

		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for username <{username}>");

		return userList[0];
	}

	public async Task<User> UpdateUsername(int userId, string newUsername, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Username.Equals(newUsername));

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 0)
			throw new RepositoryException($"Username <{newUsername}> not available");

		queryable = _dbContext.Users
			.Where(x => x.Id == userId);
		userList = await queryable.ToListAsync(cancellationToken: cancellationToken);

		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.Username, newUsername)
		, cancellationToken);

		return userList[0];
	}

	public async Task<User> DeleteUser(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		User user = userList[0];
		_dbContext.Remove(user);
		return user;
	}

	public async Task<string?> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		return userList[0].PropicPath;
	}

	public async Task<string?> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default) {
		User user = await GetUserFromUsername(username, cancellationToken);

		return user.PropicPath;
	}

	public async Task<User> DeleteImage(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.PropicPath, string.Empty)
		, cancellationToken);

		return userList[0];
	}
}
