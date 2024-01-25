using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Repository;
public class PdfFilesRepository : IPdfFilesRepository {
	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public PdfFilesRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default) {
		PdfFile pdfFile = new PdfFile {
			MusicalScoreId = pdfFileDto.MusicalScoreId,
			Path = pdfFileDto.Path,
			UploadDate = pdfFileDto.UploadDate,
			Publisher = pdfFileDto.Publisher,
			CopyrightId = pdfFileDto.CopyrightId,
			IsUrtext = pdfFileDto.IsUrtext,
			UserId = pdfFileDto.UserId,
			Comments = pdfFileDto.Comments
		};

		await _dbContext.AddAsync(pdfFile, cancellationToken);
	}

	public async Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default) {
		PdfFile pdfFile = await GetUnique(id, cancellationToken);

		_dbContext.Remove(pdfFile);
		return pdfFile;
	}

	public async Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default) {
		List<PdfFile> pdfFileList = await  _dbContext.PdfFiles
			.Where(x => x.MusicalScoreId == scoreId)
			.ToListAsync(cancellationToken);
		
		return pdfFileList;
	}

	public async Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default) {
		return await GetUnique(id, cancellationToken);
	}

	public async Task UpdatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default) {
		
		var queryable = GetQueryable(pdfFileDto.Path);

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.Path, pdfFileDto.Path)
			.SetProperty(x => x.UploadDate, pdfFileDto.UploadDate)
			.SetProperty(x => x.Publisher, pdfFileDto.Publisher)
			.SetProperty(x => x.CopyrightId, pdfFileDto.CopyrightId)
			.SetProperty(x => x.IsUrtext, pdfFileDto.IsUrtext)
			.SetProperty(x => x.UserId, pdfFileDto.UserId)
			.SetProperty(x => x.Comments, pdfFileDto.Comments)
		, cancellationToken: cancellationToken);
	}

	private IQueryable<PdfFile> GetQueryable(int id) {
		return _dbContext.PdfFiles
			.Where(x => x.Id == id);
	}
	
	private IQueryable<PdfFile> GetQueryable(string path) {
		return _dbContext.PdfFiles
			.Where(x => x.Path.Equals(path));
	}

	private async Task<PdfFile> GetUnique(int id, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.PdfFiles
			.Where(x => x.Id == id);

		List<PdfFile> pdfFileList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (pdfFileList.Count != 1)
			throw new RepositoryException($"Found <{pdfFileList.Count}> pdf files for id <{id}>");

		return pdfFileList[0];
	}

	private async Task<PdfFile> GetUnique(string path, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.PdfFiles
			.Where(x => x.Path.Equals(path));

		List<PdfFile> pdfFileList = await queryable.ToListAsync(cancellationToken);
		if (pdfFileList.Count != 1)
			throw new RepositoryException($"Found <{pdfFileList.Count}> pdf files for path <{path}>");

		return pdfFileList[0];
	}
}

