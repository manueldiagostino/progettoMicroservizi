using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IRepository {
	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	// Operazioni CRUD per Genre
	public Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	public Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default);
	public Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default);
	public Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	public Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default);

	// Operazioni CRUD per MusicalScore
	public Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default);
	public Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default);
	public Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default);
	public Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default);

	public Task<ICollection<MusicalScore>> SearchMusicalScoreFromTitle(string title, CancellationToken cancellationToken = default);
	
	// Operazioni CRUD per ScoreGenreRelationship
	public Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	public Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default);
	public Task<List<ScoreGenreRelationshipDto>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default);

	public Task<List<GenreDto>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default);
	public Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default);

	// Operazioni CRUD per PdfFile
	public Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default);
	public Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default);
	public Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default);
	public Task<PdfFile> UpdatePdfFileInfo(int fileId, PdfFileReadDto pdfFileReadDto, CancellationToken cancellationToken = default);
	public Task<PdfFile> UpdatePdfFile(int fileId, string newPath, CancellationToken cancellationToken = default);
	public Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default);
	public Task<bool> CheckMusicalScoreId(int scoreId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Copyright
	public Task CreateCopyright(CopyrightDto copyrightDto, CancellationToken cancellationToken = default);
	public Task<Copyright> GetCopyrightByName(string name, CancellationToken cancellationToken = default);
	public Task<List<Copyright>> GetAllCopyrights(CancellationToken cancellationToken = default);
	public Task<Copyright> UpdateCopyright(string oldName, string newName, CancellationToken cancellationToken = default);
	public Task<Copyright> DeleteCopyright(string name, CancellationToken cancellationToken = default);
}
