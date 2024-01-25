using System.Text.Json;
using System.Text.Json.Serialization;
using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Repository;
public class ScoreGenreRelationshipRepository : IScoreGenreRelationshipRepository {
	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public ScoreGenreRelationshipRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
		ScoreGenreRelationship relationship = new ScoreGenreRelationship {
			ScoreId = relationshipDto.ScoreId,
			GenreId = relationshipDto.GenreId
		};

		await _dbContext.AddAsync(relationship, cancellationToken);
	}

	public async Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
		ScoreGenreRelationship? scoreGenreRelationship = await _dbContext.ScoreGenreRelationships
			.FirstOrDefaultAsync(sgr => sgr.ScoreId == relationshipDto.ScoreId && sgr.GenreId == relationshipDto.GenreId, cancellationToken);

		return scoreGenreRelationship ?? throw new RepositoryException($"Found no scoreGenreRelationship for name <{JsonSerializer.Serialize(relationshipDto)}>");
	}

	public async Task<List<ScoreGenreRelationship>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default) {
		return await _dbContext.ScoreGenreRelationships.ToListAsync(cancellationToken);
	}

	public async Task<List<Genre>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default) {
		return await _dbContext.ScoreGenreRelationships
			.Where(sgr => sgr.ScoreId == scoreId)
			.Include(sgr => sgr.Genre) // navigazione alla tabella Genres
			.Select(sgr => sgr.Genre)
			.ToListAsync(cancellationToken);
	}

	public async Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default) {
		ScoreGenreRelationship relationship = await GetUnique(id, cancellationToken);

		_dbContext.Remove(relationship);
		return relationship;
	}

	private IQueryable<ScoreGenreRelationship> GetQueryable(int id) {
		return _dbContext.ScoreGenreRelationships
			.Where(x => x.Id == id);
	}

	private async Task<ScoreGenreRelationship> GetUnique(int id, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.ScoreGenreRelationships
			.Where(x => x.Id == id);

		List<ScoreGenreRelationship> relationshipList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (relationshipList.Count != 1)
			throw new RepositoryException($"Found <{relationshipList.Count}> relationships for id <{id}>");

		return relationshipList[0];
	}
}