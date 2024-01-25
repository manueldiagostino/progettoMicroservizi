using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IMusicalScoresRepository {

	// Operazioni CRUD per MusicalScore
	Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default);
	Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default);
	Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default);
}
