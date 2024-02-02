using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IScoreGenreRelationshipRepository {

	// Operazioni CRUD per ScoreGenreRelationship
	public Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	
	public Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	public Task<List<ScoreGenreRelationshipDto>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default);
	
	public Task<List<GenreDto>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default);
	public Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default);

}
