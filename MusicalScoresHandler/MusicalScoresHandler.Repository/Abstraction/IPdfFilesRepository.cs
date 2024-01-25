using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IPdfFilesRepository {

	// Operazioni CRUD per PdfFile
	Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default);
	Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default);
	Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default);
	Task UpdatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default);
	Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default);
	
}
