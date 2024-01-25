using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

using Microsoft.EntityFrameworkCore;
using GlobalUtility.Manager.Exceptions;

namespace MusicalScoresHandler.Repository.Repository;

public class GenresRepository : IGenresRepository {

	protected readonly MusicalScoresHandlerDbContext _dbContext;
	private readonly ILogger _logger;

	public GenresRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
		_dbContext = musicalScoresHandlerDbContext;
		_logger = logger;
	}

	public async Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
		Genre genre = new() {
			Name = genreDto.Name
		};

		await _dbContext.AddAsync(genre, cancellationToken);
	}

	private IQueryable<Genre> GetQueryable(string name) {
		return _dbContext.Genres
			.Where(x => x.Name.Equals(name));
	}

	private IQueryable<Genre> GetQueryable(int id) {
		return _dbContext.Genres
			.Where(x => x.Id == id);
	}

	private async Task<Genre> GetUnique(string name, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Genres
			.Where(x => x.Name.Equals(name));

		List<Genre> genreList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (genreList.Count != 1)
			throw new RepositoryException($"Found <{genreList.Count}> genres for name <{name}>");

		return genreList[0];
	}

	private async Task<Genre> GetUnique(int id, CancellationToken cancellationToken = default) {
		var queryable = _dbContext.Genres
			.Where(x => x.Id == id);

		List<Genre> genreList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (genreList.Count != 1)
			throw new RepositoryException($"Found <{genreList.Count}> genres for id <{id}>");

		return genreList[0];
	}

	public async Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default) {
		Genre genre = await GetUnique(name, cancellationToken);

		_dbContext.Remove(genre);
		return genre;
	}

	public async Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default) {
		return await _dbContext.Genres.ToListAsync(cancellationToken);
	}

	public async Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default) {
		return await GetUnique(name, cancellationToken);
	}

	public async Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default) {
		var queryable = GetQueryable(genreDto.Name);

		List<Genre> genreList = await queryable.ToListAsync(cancellationToken: cancellationToken);
		if (genreList.Count != 1)
			throw new RepositoryException($"Found <{genreList.Count}> genres for name <{genreDto.Name}>");

		await queryable.ExecuteUpdateAsync(x => x
			.SetProperty(x => x.Name, genreDto.Name)
		, cancellationToken: cancellationToken);

		return genreList[0];
	}
}
