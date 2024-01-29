using SixLabors.ImageSharp;
using UsersHandler.Repository.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;

using Microsoft.EntityFrameworkCore;
using GlobalUtility.Manager.Exceptions;
using GlobalUtility.Manager.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GlobalUtility.Kafka.Model;

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

	public async Task<User> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == bioDto.UserId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{bioDto.UserId}>");

		User user = userList[0];
		if (user.BioId != null)
			throw new RepositoryException($"User <{user.Id}> has already bio <{user.BioId}>");

		Bio bio = new() {
			UserId = bioDto.UserId,
			Text = bioDto.Text
		};

		await _dbContext.AddAsync(bio, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);

		List<Bio> bioList = await _dbContext.Biographies
			.Where(x => x.UserId == bioDto.UserId)
			.ToListAsync(cancellationToken: cancellationToken);

		if (bioList.Count != 1)
			throw new RepositoryException($"Found <{bioList.Count}> users for id <{bioDto.UserId}>");

		int bioId = bioList[0].Id;
		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.BioId, bioId)
		, cancellationToken);

		return user;
	}

	public async Task<User> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Users
			.Where(x => x.Id == bioDto.UserId);

		List<User> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{bioDto.UserId}>");

		User user = userList[0];
		if (user.BioId == null) {
			_logger.LogInformation("No bio found, creating one");
			return await CreateBioFromId(bioDto, cancellationToken);
		}

		await _dbContext.Biographies
			.Where(x => x.UserId == bioDto.UserId)
			.ExecuteUpdateAsync(x => x
				.SetProperty(x => x.Text, bioDto.Text)
			, cancellationToken);

		return user;
	}

	public async Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Biographies
			.Where(x => x.UserId == userId);

		List<Bio> bioList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (bioList.Count != 1)
			throw new RepositoryException($"Found <{bioList.Count}> bio for id <{userId}>");

		return bioList[0].Text;
	}

	public async Task<User> DeleteBioFromId(int userId, CancellationToken cancellationToken = default) {
		var queryableBio = _dbContext.Biographies
			.Where(x => x.UserId == userId);

		List<Bio> bioList = await queryableBio.ToListAsync(cancellationToken: cancellationToken);
		if (bioList.Count != 1)
			throw new RepositoryException($"Found <{bioList.Count}> bio for id <{userId}>");

		_dbContext.Remove(bioList[0]);

		var queryableUser = _dbContext.Users
			.Where(x => x.Id == userId);

		List<User> userList = await queryableUser.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		User user = userList[0];
		int? newBioId = null;

		await queryableUser
			.ExecuteUpdateAsync(x => x
				.SetProperty(x => x.BioId, newBioId)
			, cancellationToken);

		return user;
	}

	public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxes(CancellationToken ct = default) {
		return await _dbContext.TransactionalOutboxes.ToListAsync(cancellationToken: ct);
	}

	public async Task InsertTransactionalOutbox(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default) {
		await _dbContext.AddAsync(transactionalOutbox, cancellationToken);
	}
	public async Task DeleteTransactionalOutboxFromId(int id, CancellationToken cancellationToken = default) {
		var res = _dbContext.TransactionalOutboxes.Where(x => x.id == id).ToList();
		if (res.Count <= 0)
			throw new RepositoryException("res.Count() <= 0");

		_dbContext.Remove(res[0]);
		await Task.CompletedTask;
	}
}
