using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Repository;

public class UserKafkaRepository : IUserKafkaRepository {
	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public UserKafkaRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	private async Task<UserKafka> GetUnique(int userId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.UsersKafka
			.Where(x => x.UserId == userId);

		List<UserKafka> userList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (userList.Count != 1)
			throw new RepositoryException($"Found <{userList.Count}> users for id <{userId}>");

		return userList[0];
	}

	public async Task<bool> CheckUserId(int userId, CancellationToken cancellationToken = default) {
		try {
			await GetUnique(userId, cancellationToken);
			return true;
		} catch { }
		return false;
	}

	public async Task<UserKafka> DeleteUser(int userId, CancellationToken cancellationToken = default) {
		UserKafka user = await GetUnique(userId, cancellationToken);

		_dbContext.Remove(user);
		return user;
	}

	public async Task InsertUser(UserKafka userKafka, CancellationToken cancellationToken = default) {
		await _dbContext.AddAsync(userKafka, cancellationToken);
	}

	public async Task<UserKafka> UpdateUser(UserKafka userKafka, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.UsersKafka
			.Where(x => x.UserId==userKafka.UserId);

		int changes = await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.Username, userKafka.Username)
			.SetProperty(x => x.Name, userKafka.Name)
			.SetProperty(x => x.Surname, userKafka.Surname)
		, cancellationToken);

		if (changes != 1)
			throw new RepositoryException($"Found <{changes}> users for id <{userKafka.UserId}>");

		return (await queryable.ToListAsync(cancellationToken: cancellationToken))[0];
	}
}
