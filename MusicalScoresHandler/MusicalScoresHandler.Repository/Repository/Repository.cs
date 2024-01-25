using Microsoft.Extensions.Logging;
using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicalScoresHandler.Repository.Repository {
	public class Repository : IRepository {
		protected readonly MusicalScoresHandlerDbContext _dbContext;
		private readonly ILogger _logger;
		private readonly IGenresRepository _genresRepository;
		private readonly IMusicalScoresRepository _musicalScoresRepository;
		private readonly IPdfFilesRepository _pdfFilesRepository;
		private readonly IScoreGenreRelationshipRepository _scoreGenreRelationshipRepository;
		private readonly ICopyrightRepository _copyrightRepository;

		public Repository(
			MusicalScoresHandlerDbContext musicalScoresHandlerDbContext,
			ILogger<Repository> logger,
			IGenresRepository genreRepository,
			IMusicalScoresRepository musicalScoresRepository,
			IPdfFilesRepository pdfFilesRepository,
			IScoreGenreRelationshipRepository scoreGenreRelationshipRepository,
			ICopyrightRepository copyrightRepository) {
			_dbContext = musicalScoresHandlerDbContext;
			_logger = logger;
			_genresRepository = genreRepository;
			_musicalScoresRepository = musicalScoresRepository;
			_pdfFilesRepository = pdfFilesRepository;
			_scoreGenreRelationshipRepository = scoreGenreRelationshipRepository;
			_copyrightRepository = copyrightRepository;
		}

		public async Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
			await _genresRepository.CreateGenre(genreDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task CreateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			await _musicalScoresRepository.CreateMusicalScore(musicalScoreDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task CreatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default) {
			await _pdfFilesRepository.CreatePdfFile(pdfFileDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task CreateScoreGenreRelationship(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
			await _scoreGenreRelationshipRepository.CreateScoreGenreRelationship(relationshipDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default) {
			var genre = await _genresRepository.DeleteGenre(name, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return genre;
		}

		public async Task<MusicalScore> DeleteMusicalScore(int id, CancellationToken cancellationToken = default) {
			var musicalScore = await _musicalScoresRepository.DeleteMusicalScore(id, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return musicalScore;
		}

		public async Task<PdfFile> DeletePdfFile(int id, CancellationToken cancellationToken = default) {
			var pdfFile = await _pdfFilesRepository.DeletePdfFile(id, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return pdfFile;
		}

		public async Task<ScoreGenreRelationship> DeleteScoreGenreRelationship(int id, CancellationToken cancellationToken = default) {
			var scoreGenreRelationship = await _scoreGenreRelationshipRepository.DeleteScoreGenreRelationship(id, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return scoreGenreRelationship;
		}

		public async Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default) {
			return await _genresRepository.GetAllGenres(cancellationToken);
		}

		public async Task<List<MusicalScore>> GetAllMusicalScores(CancellationToken cancellationToken = default) {
			return await _musicalScoresRepository.GetAllMusicalScores(cancellationToken);
		}

		public async Task<List<ScoreGenreRelationship>> GetAllScoreGenreRelationships(CancellationToken cancellationToken = default) {
			return await _scoreGenreRelationshipRepository.GetAllScoreGenreRelationships(cancellationToken);
		}

		public async Task<List<Genre>> GetAllScoreGenres(int scoreId, CancellationToken cancellationToken = default) {
			return await _scoreGenreRelationshipRepository.GetAllScoreGenres(scoreId, cancellationToken);
		}

		public async Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default) {
			return await _genresRepository.GetGenreByName(name, cancellationToken);
		}

		public async Task<MusicalScore> GetMusicalScoreById(int id, CancellationToken cancellationToken = default) {
			return await _musicalScoresRepository.GetMusicalScoreById(id, cancellationToken);
		}

		public async Task<int> GetMusicalScoreId(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			return await _musicalScoresRepository.GetMusicalScoreId(musicalScoreDto, cancellationToken);
		}

		public async Task<PdfFile> GetPdfFileById(int id, CancellationToken cancellationToken = default) {
			return await _pdfFilesRepository.GetPdfFileById(id, cancellationToken);
		}

		public async Task<List<PdfFile>> GetPdfFilesForMusicalScore(int scoreId, CancellationToken cancellationToken = default) {
			return await _pdfFilesRepository.GetPdfFilesForMusicalScore(scoreId, cancellationToken);
		}

		public async Task<ScoreGenreRelationship> GetScoreGenreRelationshipID(ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
			return await _scoreGenreRelationshipRepository.GetScoreGenreRelationshipID(relationshipDto, cancellationToken);
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			return await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
			Genre genre = await _genresRepository.UpdateGenre(genreDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return genre;
		}

		public async Task<MusicalScore> UpdateMusicalScore(MusicalScoreDto musicalScoreDto, CancellationToken cancellationToken = default) {
			MusicalScore musicalScore = await _musicalScoresRepository.UpdateMusicalScore(musicalScoreDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return musicalScore;
		}

		public async Task UpdatePdfFile(PdfFileDto pdfFileDto, CancellationToken cancellationToken = default) {
			await _pdfFilesRepository.UpdatePdfFile(pdfFileDto, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
