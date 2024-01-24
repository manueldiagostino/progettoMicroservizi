using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IMusicalScoresRepository {

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	// Operazioni CRUD per Genre
	Task CreateGenre(Genre genre, CancellationToken cancellationToken = default);
	Task<Genre?> GetGenreById(int id, CancellationToken cancellationToken = default);
	Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default);
	Task UpdateGenre(Genre genre, CancellationToken cancellationToken = default);
	Task DeleteGenre(int id, CancellationToken cancellationToken = default);

	// Operazioni CRUD per MusicalScore
	Task CreateMusicalScore(MusicalScore musicalScore, CancellationToken cancellationToken = default);
	Task<MusicalScore?> GetMusicalScoreById(int id, CancellationToken cancellationToken = default);
	Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default);
	Task UpdateMusicalScore(MusicalScore musicalScore, CancellationToken cancellationToken = default);
	Task DeleteMusicalScore(int id, CancellationToken cancellationToken = default);

	// Operazioni CRUD per PdfFile
	Task CreatePdfFile(PdfFile pdfFile, CancellationToken cancellationToken = default);
	Task<PdfFile?> GetPdfFileById(int id, CancellationToken cancellationToken = default);
	Task<List<PdfFile>> GetAllPdfFiles(CancellationToken cancellationToken = default);
	Task UpdatePdfFile(PdfFile pdfFile, CancellationToken cancellationToken = default);
	Task DeletePdfFile(int id, CancellationToken cancellationToken = default);

	// Operazioni CRUD per ScoreGenreRelationship
	Task CreateScoreGenreRelationship(ScoreGenreRelationship relationship, CancellationToken cancellationToken = default);
	Task<List<ScoreGenreRelationship>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default);
	Task DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default);

	// Operazioni specifiche per relazioni
	Task<List<MusicalScore>> GetMusicalScoresByGenre(int genreId, CancellationToken cancellationToken = default);
	Task<List<Genre>> GetGenresByScore(int scoreId, CancellationToken cancellationToken = default);
}
