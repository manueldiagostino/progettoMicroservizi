using GlobalUtility.Manager.Exceptions;
using GlobalUtility.Manager.Operations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AuthorsHandler.ClientHttp.Abstraction;
using UsersHandler.ClientHttp.Abstraction;

namespace MusicalScoresHandler.Business.Business {
	public class Business : IBusiness {
		private readonly ILogger _logger;
		private readonly IRepository _repository;

		private readonly IAuthorKafkaRepository _authorKafkaRepository;
		private readonly IUserKafkaRepository _userKafkaRepository;
		private readonly IAuthorsHandlerClientHttp _authorsClientHttp;
		private readonly IUsersHandlerClientHttp _usersClientHttp;

		public Business(
		ILogger<Business> logger,
		IRepository repository,
		IAuthorKafkaRepository authorKafkaRepository,
		IUserKafkaRepository userKafkaRepository,
		IAuthorsHandlerClientHttp authorsClientHttp,
		IUsersHandlerClientHttp usersClientHttp) {

			_logger = logger;
			_repository = repository;
			_authorKafkaRepository = authorKafkaRepository;
			_userKafkaRepository = userKafkaRepository;
			_authorsClientHttp = authorsClientHttp;
			_usersClientHttp = usersClientHttp;
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			return await _repository.SaveChangesAsync(cancellationToken);
		}

		// Operazioni CRUD per Genre
		public async Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
			if (genreDto == null)
				throw new BusinessException("genreDto == null", nameof(genreDto));

			await _repository.CreateGenre(genreDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Created Genre <{genreDto.Name}>");
		}

		public async Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default) {
			if (string.IsNullOrWhiteSpace(name))
				throw new BusinessException("IsNullOrWhiteSpace(name)", nameof(name));

			return await _repository.GetGenreByName(name, cancellationToken);
		}

		public async Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default) {
			return await _repository.GetAllGenres(cancellationToken);
		}

		public async Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
			if (genreDto == null)
				throw new BusinessException("genreDto == null", nameof(genreDto));

			var updatedGenre = await _repository.UpdateGenre(genreDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Updated Genre <{genreDto.Name}> to {updatedGenre.Name}");
			return updatedGenre;
		}

		public async Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default) {
			if (string.IsNullOrWhiteSpace(name))
				throw new BusinessException("IsNullOrWhiteSpace(name)", nameof(name));

			var deletedGenre = await _repository.DeleteGenre(name, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Deleted Genre <{deletedGenre.Name}>");
			return deletedGenre;
		}

		private async Task CheckAuthorWithHttp(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			_logger.LogInformation($"Failed checking musicalScoreDto.AuthorId from cache, trying with http...");
			HttpResponseMessage response = await _authorsClientHttp.GetAuthorFromId(musicalScoreDto.AuthorId, cancellationToken);

			if (!response.IsSuccessStatusCode)
				throw new BusinessException($"No such author for id <{musicalScoreDto.AuthorId}>");

			string authorString = await response.Content.ReadAsStringAsync(cancellationToken);
			_logger.LogInformation($"Response string <{authorString}>");

			var deserializeOptions = new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			AuthorKafkaDto? authorDto = JsonSerializer.Deserialize<AuthorKafkaDto>(authorString, deserializeOptions);
			if (authorDto == null)
				throw new BusinessException($"CheckAuthorWithHttp: authorDto == null");

			_logger.LogInformation($"Deserialized AuthorKafkaDto <{JsonSerializer.Serialize(authorDto)}>");

			AuthorKafka author = new() {
				AuthorId = authorDto.AuthorId,
				Name = authorDto.Name,
				Surname = authorDto.Surname
			};

			await _authorKafkaRepository.InsertAuthor(author, cancellationToken);
			await _authorKafkaRepository.SaveChangesAsync(cancellationToken);
			_logger.LogInformation($"Saved AuthorKafka into DBMS");
		}

		// Operazioni CRUD per MusicalScore
		public async Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			if (musicalScoreDto == null)
				throw new BusinessException("musicalScoreDto == null");

			// Check validity of the autorId
			if (!await _authorKafkaRepository.CheckAuthorId(musicalScoreDto.AuthorId, cancellationToken))
				await CheckAuthorWithHttp(musicalScoreDto, cancellationToken);

			await _repository.CreateMusicalScore(musicalScoreDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Created MusicalScore <{JsonSerializer.Serialize(musicalScoreDto)}>");
		}

		public async Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default) {
			if (id <= 0)
				throw new BusinessException("id <= 0");

			return await _repository.GetMusicalScoreById(id, cancellationToken);
		}

		public async Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			if (musicalScoreDto == null)
				throw new BusinessException("musicalScoreDto == null", nameof(musicalScoreDto));

			return await _repository.GetMusicalScoreId(musicalScoreDto, cancellationToken);
		}

		public async Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default) {
			return await _repository.GetAllMusicalScores(cancellationToken);
		}

		public async Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			if (musicalScoreDto == null)
				throw new BusinessException("musicalScoreDto == null", nameof(musicalScoreDto));

			var updatedMusicalScore = await _repository.UpdateMusicalScore(musicalScoreDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Updated MusicalScore <{JsonSerializer.Serialize(updatedMusicalScore)}> to <{JsonSerializer.Serialize(musicalScoreDto)}>");
			return updatedMusicalScore;
		}

		public async Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default) {
			if (id <= 0)
				throw new BusinessException("id <= 0", nameof(id));

			var deletedMusicalScore = await _repository.DeleteMusicalScore(id, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Deleted MusicalScore <{JsonSerializer.Serialize(deletedMusicalScore)}>");
			return deletedMusicalScore;
		}

		// Operazioni CRUD per ScoreGenreRelationship
		public async Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
			if (relationshipDto == null)
				throw new BusinessException("relationshipDto == null", nameof(relationshipDto));

			await _repository.CreateScoreGenreRelationship(relationshipDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Created ScoreGenreRelationship <{JsonSerializer.Serialize(relationshipDto)}>");
		}

		public async Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
			if (relationshipDto == null)
				throw new BusinessException("relationshipDto == null", nameof(relationshipDto));

			return await _repository.GetScoreGenreRelationshipID(relationshipDto, cancellationToken);
		}

		public async Task<List<ScoreGenreRelationshipDto>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default) {
			return await _repository.GetAllScoreGenreRelationships(cancellationToken);
		}

		public async Task<List<GenreDto>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default) {
			if (scoreId <= 0)
				throw new BusinessException("id <= 0", nameof(scoreId));

			return await _repository.GetAllScoreGenres(scoreId, cancellationToken);
		}

		public async Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default) {
			if (id <= 0)
				throw new BusinessException("id <= 0", nameof(id));

			var deletedRelationship = await _repository.DeleteScoreGenreRelationship(id, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Deleted ScoreGenreRelationship <{JsonSerializer.Serialize(deletedRelationship)}>");
			return deletedRelationship;
		}

		// Operazioni CRUD per PdfFile
		private async Task CheckUserWithHttp(PdfFileReadDto pdfFileDto, CancellationToken cancellationToken = default) {
			_logger.LogInformation($"Failed checking musicalScoreDto.AuthorId from cache, trying with http...");
			HttpResponseMessage response = await _usersClientHttp.GetUserFromId(pdfFileDto.UserId, cancellationToken);

			if (!response.IsSuccessStatusCode)
				throw new BusinessException($"No such user for id <{pdfFileDto.UserId}>");

			string userString = await response.Content.ReadAsStringAsync(cancellationToken);
			_logger.LogInformation($"Response string <{userString}>");

			var deserializeOptions = new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			UserKafkaDto? userDto = JsonSerializer.Deserialize<UserKafkaDto>(userString, deserializeOptions);
			if (userDto == null)
				throw new BusinessException($"CheckUserWithHttp: userDto == null");

			_logger.LogInformation($"Deserialized UserKafkaDto <{JsonSerializer.Serialize(userDto)}>");

			UserKafka user = new() {
				UserId = userDto.UserId,
				Username = userDto.Username,
				Name = userDto.Name,
				Surname = userDto.Surname
			};

			await _userKafkaRepository.InsertUser(user, cancellationToken);
			await _userKafkaRepository.SaveChangesAsync(cancellationToken);
			_logger.LogInformation($"Saved UserKafka into DBMS");
		}

		public async Task CreatePdfFile(PdfFileReadDto pdfFileReadDto, IFormFile file, CancellationToken cancellationToken = default) {
			if (pdfFileReadDto == null)
				throw new BusinessException("pdfFileReadDto == null", nameof(pdfFileReadDto));
			if (file == null)
				throw new BusinessException("file == null", nameof(file));
			if (!await _repository.CheckMusicalScoreId(pdfFileReadDto.MusicalScoreId, cancellationToken))
				throw new BusinessException($"Error while creating new PdfFile: no such MusicalScore for id <{pdfFileReadDto.MusicalScoreId}>");

			// Check validity of the userId
			if (!await _userKafkaRepository.CheckUserId(pdfFileReadDto.UserId, cancellationToken))
				await CheckUserWithHttp(pdfFileReadDto, cancellationToken);

			string? relativePath = Files.SaveFileToDir(Path.Combine("PdfScores"), file) ?? throw new BusinessException("SaveFileToDisk returned path == null");

			PdfFileDto pdfFileDto = new PdfFileDto {
				MusicalScoreId = pdfFileReadDto.MusicalScoreId,
				Path = relativePath,
				UploadDate = DateTime.UtcNow,
				Publisher = pdfFileReadDto.Publisher,
				CopyrightId = pdfFileReadDto.CopyrightId,
				IsUrtext = pdfFileReadDto.IsUrtext,
				Comments = pdfFileReadDto.Comments
			};

			await _repository.CreatePdfFile(pdfFileDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Created PdfFile <{JsonSerializer.Serialize(pdfFileDto)}>");
		}

		public async Task<PdfFile> UpdatePdfFile(int fileId, IFormFile file, CancellationToken cancellationToken = default) {
			if (fileId <= 0)
				throw new BusinessException("fileId <= 0", nameof(fileId));
			if (file == null)
				throw new BusinessException("file == null", nameof(file));

			string relativePath = Files.SaveFileToDir(Path.Combine("PdfScores"), file);
			_logger.LogInformation($"File saved into <{relativePath}>");

			return await _repository.UpdatePdfFile(fileId, relativePath, cancellationToken);
		}

		public async Task<string?> GetPdfFileById(int id, CancellationToken cancellationToken = default) {
			if (id <= 0)
				throw new BusinessException("id <= 0", nameof(id));

			PdfFile pdfFile = await _repository.GetPdfFileById(id, cancellationToken);
			string? relativePath = pdfFile.Path;

			return Files.GetAbsolutePath(relativePath);
		}

		public async Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default) {
			if (scoreId <= 0)
				throw new BusinessException("scoreId <= 0", nameof(scoreId));

			return await _repository.GetPdfFilesForMusicalScore(scoreId, cancellationToken);
		}

		public async Task<PdfFile> UpdatePdfFileInfo(int fileId, PdfFileReadDto pdfFileReadDto, CancellationToken cancellationToken = default) {
			if (fileId <= 0)
				throw new BusinessException("fileId <= 0", nameof(fileId));

			var updatedPdfFile = await _repository.UpdatePdfFileInfo(fileId, pdfFileReadDto, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Updated PdfFile <{JsonSerializer.Serialize(updatedPdfFile)}> to <{JsonSerializer.Serialize(pdfFileReadDto)}>");
			return updatedPdfFile;
		}

		public async Task<PdfFile> DeletePdfFile(int fileId, CancellationToken cancellationToken = default) {
			if (fileId <= 0)
				throw new BusinessException("userId <= 0", nameof(fileId));

			var deletedPdfFile = await _repository.DeletePdfFile(fileId, cancellationToken);
			await SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Deleted PdfFile <{JsonSerializer.Serialize(deletedPdfFile)}>");
			return deletedPdfFile;
		}

		public async Task CreateCopyright(CopyrightDto copyrightDto, CancellationToken cancellationToken = default) {
			await _repository.CreateCopyright(copyrightDto, cancellationToken);
			await _repository.SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Created Copyright <{JsonSerializer.Serialize(copyrightDto)}>");
		}

		public async Task<Copyright> GetCopyrightByName(string name, CancellationToken cancellationToken = default) {
			if (string.IsNullOrEmpty(name))
				throw new BusinessException("string.IsNullOrEmpty(name)", nameof(name));

			return await _repository.GetCopyrightByName(name, cancellationToken);
		}

		public async Task<List<Copyright>> GetAllCopyrights(CancellationToken cancellationToken = default) {
			return await _repository.GetAllCopyrights(cancellationToken);
		}

		public async Task<Copyright> UpdateCopyright(string oldName, string newName, CancellationToken cancellationToken = default) {
			if (string.IsNullOrEmpty(oldName))
				throw new BusinessException("string.IsNullOrEmpty(name)", nameof(oldName));
			if (string.IsNullOrEmpty(newName))
				throw new BusinessException("string.IsNullOrEmpty(name)", nameof(newName));

			var copyright = await _repository.UpdateCopyright(oldName, newName, cancellationToken);
			await _repository.SaveChangesAsync(cancellationToken);

			return copyright;
		}

		public async Task<Copyright> DeleteCopyright(string name, CancellationToken cancellationToken = default) {
			if (string.IsNullOrEmpty(name))
				throw new BusinessException("string.IsNullOrEmpty(name)", nameof(name));
			
			var copyright = await _repository.DeleteCopyright(name, cancellationToken);
			await _repository.SaveChangesAsync(cancellationToken);

			return copyright;
		}

		public async Task<ICollection<MusicalScore>> SearchMusicalScoreFromTitle(string title, CancellationToken cancellationToken) {
			if (string.IsNullOrEmpty(title))
				throw new BusinessException("string.IsNullOrEmpty(name)", nameof(title));

			return await _repository.SearchMusicalScoreFromTitle(title, cancellationToken);
		}
	}
}
