using System.Text.Json;
using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
namespace MusicalScoresHandler.Repository;

public class MusicalScoresRepository : IMusicalScoresRepository {

	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public MusicalScoresRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<MusicalScoresRepository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
		MusicalScore musicalScore = new() {
			Title = musicalScoreDto.Title,
			AuthorId = musicalScoreDto.AuthorId,
			Opus = musicalScoreDto.Opus,
			Alias = musicalScoreDto.Alias,
			Description = musicalScoreDto.Description,
			YearOfComposition = musicalScoreDto.YearOfComposition
		};

		await _dbContext.AddAsync(musicalScore, cancellationToken);
	}

	public async Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default) {
		return await GetUnique(id, cancellationToken);
	}

	public async Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
		List<MusicalScore> musicalScoreList = await _dbContext.MusicalScores
			.Where(x =>
				x.Title.Equals(musicalScoreDto.Title) &&
				x.AuthorId == musicalScoreDto.AuthorId)
			.ToListAsync(cancellationToken: cancellationToken);

		if (musicalScoreList.Count != 1) {
			throw new RepositoryException($"Found <{musicalScoreList.Count}> genres for <{JsonSerializer.Serialize(musicalScoreDto)}>");
		}

		return musicalScoreList[0].Id;
	}


	public async Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default) {
		return await _dbContext.MusicalScores.ToListAsync(cancellationToken);
	}

	public async Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
		int musicalScoreId = await GetMusicalScoreId(musicalScoreDto, cancellationToken);

		var queryable = GetQueryable(musicalScoreId);

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.Title, musicalScoreDto.Title)
			.SetProperty(x => x.AuthorId, musicalScoreDto.AuthorId)
			.SetProperty(x => x.Opus, musicalScoreDto.Opus)
			.SetProperty(x => x.Alias, musicalScoreDto.Alias)
			.SetProperty(x => x.Description, musicalScoreDto.Description)
			.SetProperty(x => x.YearOfComposition, musicalScoreDto.YearOfComposition)
		, cancellationToken);

		return (await queryable.ToListAsync(cancellationToken: cancellationToken))[0];
	}

	public async Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default) {
		var musicalScore = await GetMusicalScoreById(id, cancellationToken);

		_dbContext.Remove(musicalScore);
		return musicalScore;
	}

	private IQueryable<MusicalScore> GetQueryable(int id) {
		return _dbContext.MusicalScores
			.Where(x => x.Id == id);
	}

	private async Task<MusicalScore> GetUnique(int id, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.MusicalScores
			.Where(x => x.Id == id);

		List<MusicalScore> musicalScoreList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (musicalScoreList.Count != 1)
			throw new RepositoryException($"Found <{musicalScoreList.Count}> musical scores for id <{id}>");

		return musicalScoreList[0];
	}

	public async Task<bool> CheckMusicalScoreId(int scoreId, CancellationToken cancellationToken = default) {
		try {
			await GetUnique(scoreId, cancellationToken);
			return true;
		} catch { }
		return false;
	}
}
