using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
namespace MusicalScoresHandler.Repository;

public class MusicalScoresRepository : IMusicalScoresRepository {
	public Task CreateGenre(Genre genre, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task CreateMusicalScore(MusicalScore musicalScore, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task CreatePdfFile(PdfFile pdfFile, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task CreateScoreGenreRelationship(ScoreGenreRelationship relationship, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task DeleteGenre(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task DeleteMusicalScore(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task DeletePdfFile(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<PdfFile>> GetAllPdfFiles(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<ScoreGenreRelationship>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<Genre?> GetGenreById(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<Genre>> GetGenresByScore(int scoreId, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<MusicalScore?> GetMusicalScoreById(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<List<MusicalScore>> GetMusicalScoresByGenre(int genreId, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<PdfFile?> GetPdfFileById(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task UpdateGenre(Genre genre, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task UpdateMusicalScore(MusicalScore musicalScore, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task UpdatePdfFile(PdfFile pdfFile, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}
}
