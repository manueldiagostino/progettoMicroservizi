using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IGenresRepository {

	// Operazioni CRUD per Genre
	public Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	public Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default);
	public Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default);
	public Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	public Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default);
}
