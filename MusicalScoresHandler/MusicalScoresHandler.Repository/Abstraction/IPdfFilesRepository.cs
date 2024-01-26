using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IPdfFilesRepository {

	// Operazioni CRUD per PdfFile
	public Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default);
	public Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default);
	public Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default);
	public Task<PdfFile> UpdatePdfFileInfo(int fileId, PdfFileReadDto pdfFileReadDto, CancellationToken cancellationToken = default);
	public Task<PdfFile> UpdatePdfFile(int fileId, string newPath, CancellationToken cancellationToken = default);
	public Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default);
	
}
