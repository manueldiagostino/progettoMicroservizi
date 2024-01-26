using System.Text.Json;
using GlobalUtility.Manager.Operations;
using Microsoft.AspNetCore.Mvc;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Api.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class PdfFileController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger<PdfFileController> _logger;

		public PdfFileController(IBusiness business, ILogger<PdfFileController> logger) {
			_business = business;
			_logger = logger;
		}

		private bool ValidateFile(IFormFile file) {

			var fileExtension = Path.GetExtension(file.FileName);
			var allowedExtensions = new[] { ".pdf" };

			if (!allowedExtensions.Contains(fileExtension.ToLower())) {
				return false;
			}
			
			return true;
		}

		[HttpPost(Name = "CreatePdfFile")]
		public async Task<ActionResult> CreatePdfFile([FromQuery] PdfFileReadDto pdfFileDto, [FromForm] IFormFile file) {
			if (file == null || file.Length == 0) {
				return BadRequest("File not valid");
			}

			if (!ValidateFile(file)) {
				return BadRequest($"File extension not allowed");
			}


			try {
				await _business.CreatePdfFile(pdfFileDto, file);
				_logger.LogInformation($"Created PDF File: {JsonSerializer.Serialize(pdfFileDto)}");
				return Ok($"Created PDF File: {JsonSerializer.Serialize(pdfFileDto)}");
			} catch (Exception e) {
				_logger.LogError($"Error creating PDF File: {e}");
				return BadRequest($"Error creating PDF File: {e.Message}");
			}
		}

		[HttpGet(Name = "GetPdfFileById")]
		public async Task<ActionResult> GetPdfFileById([FromQuery] int id) {
			try {

				string? filePath = await _business.GetPdfFileById(id);

				if (string.IsNullOrEmpty(filePath))
					return NotFound($"No pdf for id <{id}>");

				if (!System.IO.File.Exists(filePath)) {
					return NotFound($"no file for path <{filePath}>");
				}

				string contentType = "application/pdf";

				return PhysicalFile(filePath, contentType, Path.GetFileName(filePath));

			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpGet(Name = "GetPdfFilesForMusicalScore")]
		public async Task<ActionResult> GetPdfFilesForMusicalScore(int scoreId) {
			try {
				var pdfFiles = await _business.GetPdfFilesForMusicalScore(scoreId);
				_logger.LogInformation($"Retrieved {pdfFiles.Count} PDF Files for Musical Score Id: {scoreId}");

				List<string> paths = new List<string>();
				foreach (var pdf in pdfFiles)
					paths.Add(Files.GetAbsolutePath(pdf.Path)??"");

				FileStreamResult fileStreamResult = Files.GenerateZipArchive(paths);
				return fileStreamResult;
			} catch (Exception e) {
				_logger.LogError($"Error getting PDF Files: {e}");
				return BadRequest($"Error getting PDF Files: {e.Message}");
			}
		}

		[HttpPut(Name = "UpdatePdfFileInfo")]
		public async Task<ActionResult<PdfFile>> UpdatePdfFileInfo([FromQuery] int pdfFileId, [FromQuery] PdfFileReadDto pdfFileReadDto) {
			try {
				var updatedPdfFile = await _business.UpdatePdfFileInfo(pdfFileId, pdfFileReadDto);
				_logger.LogInformation($"Updated PDF File with Id: {updatedPdfFile.Id}");
				return Ok($"Updated PDF File with Id: {updatedPdfFile.Id}");
			} catch (Exception e) {
				_logger.LogError($"Error updating PDF File: {e}");
				return BadRequest($"Error updating PDF File: {e.Message}");
			}
		}

		[HttpPut(Name = "UpdatePdfFile")]
		public async Task<ActionResult<PdfFile>> UpdatePdfFile([FromQuery] int pdfFileId, [FromForm] IFormFile file) {
			if (file == null || file.Length == 0) {
				return BadRequest("File not valid");
			}

			if (!ValidateFile(file)) {
				return BadRequest($"File extension not allowed");
			}

			try {
				var updatedPdfFile = await _business.UpdatePdfFile(pdfFileId, file);
				_logger.LogInformation($"Updated PDF File with Id: {updatedPdfFile.Id}");
				return Ok($"Updated PDF File with Id: {updatedPdfFile.Id}");
			} catch (Exception e) {
				_logger.LogError($"Error updating PDF File: {e}");
				return BadRequest($"Error updating PDF File: {e.Message}");
			}
		}

		[HttpDelete(Name = "DeletePdfFile")]
		public async Task<ActionResult<PdfFile>> DeletePdfFile(int id) {
			try {
				var deletedPdfFile = await _business.DeletePdfFile(id);
				_logger.LogInformation($"Deleted PDF File with Id: {deletedPdfFile.Id}");
				return Ok(deletedPdfFile);
			} catch (Exception e) {
				_logger.LogError($"Error deleting PDF File: {e}");
				return BadRequest($"Error deleting PDF File: {e.Message}");
			}
		}
	}
}
