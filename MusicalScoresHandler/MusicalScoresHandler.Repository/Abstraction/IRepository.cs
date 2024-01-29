using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IRepository {
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	// Operazioni CRUD per Genre
	Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default);
	Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default);
	Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default);

	// Operazioni CRUD per MusicalScore
	Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default);
	Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default);
	Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default);
	
	// Operazioni CRUD per ScoreGenreRelationship
	Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	Task<List<ScoreGenreRelationshipDto>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default);

	Task<List<GenreDto>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default);
	Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default);

	// Operazioni CRUD per PdfFile
	Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default);
	Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default);
	Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default);
	Task<PdfFile> UpdatePdfFileInfo(int fileId, PdfFileReadDto pdfFileReadDto, CancellationToken cancellationToken = default);
	Task<PdfFile> UpdatePdfFile(int fileId, string newPath, CancellationToken cancellationToken = default);
	Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default);
	Task<bool> CheckMusicalScoreId(int scoreId, CancellationToken cancellationToken = default);
}
