using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IMusicalScoresRepository {

	// Operazioni CRUD per MusicalScore
	public Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default);
	public Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default);
	public Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default);
	public Task<bool> CheckMusicalScoreId(int scoreId, CancellationToken cancellationToken = default);
	public Task<ICollection<MusicalScore>> SearchMusicalScoreFromTitle(string title, CancellationToken cancellationToken);
}
