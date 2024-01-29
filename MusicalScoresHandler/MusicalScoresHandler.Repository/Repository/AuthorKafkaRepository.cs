using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Repository;

public class AuthorKafkaRepository : IAuthorKafkaRepository {
	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public AuthorKafkaRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken = default) {
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<AuthorKafka> DeleteAuthor(int authorId, CancellationToken cancellationToken = default) {
		AuthorKafka author = await GetUnique(authorId, cancellationToken);
		_dbContext.Remove(author);

		return author;
	}

	public async Task InsertAuthor(AuthorKafka authorKafka, CancellationToken cancellationToken = default) {
		await _dbContext.AddAsync(authorKafka, cancellationToken);
	}

	public async Task<AuthorKafka> UpdateAuthor(AuthorKafka authorKafka, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.AuthorsKafka
			.Where(x => x.AuthorId==authorKafka.AuthorId);

		int changes = await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.AuthorId, authorKafka.AuthorId)
			.SetProperty(x => x.Name, authorKafka.Name)
			.SetProperty(x => x.Surname, authorKafka.Surname)
		, cancellationToken);

		if (changes != 1)
			throw new RepositoryException($"Found <{changes}> authors for id <{authorKafka.AuthorId}>");

		return (await queryable.ToListAsync(cancellationToken: cancellationToken))[0];
	}

	private async Task<AuthorKafka> GetUnique(int authorId, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.AuthorsKafka
			.Where(x => x.AuthorId == authorId);

		List<AuthorKafka> authorsList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (authorsList.Count != 1)
			throw new RepositoryException($"Found <{authorsList.Count}> authors for id <{authorId}>");

		return authorsList[0];
	}

	public async Task<bool> CheckAuthorId(int authorId, CancellationToken cancellationToken = default) {
		try {
			await GetUnique(authorId, cancellationToken);
			return true;
		} catch { }
		return false;
	}
}
