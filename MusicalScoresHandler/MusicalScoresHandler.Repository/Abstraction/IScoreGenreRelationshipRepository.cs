using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IScoreGenreRelationshipRepository {

	// Operazioni CRUD per ScoreGenreRelationship
	Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	
	Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	Task<List<ScoreGenreRelationship>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default);
	
	Task<List<Genre>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default);
	Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default);

}
