using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IGenresRepository {

	// Operazioni CRUD per Genre
	Task CreateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	Task<Genre> GetGenreByName(string name, CancellationToken cancellationToken = default);
	Task<List<Genre>> GetAllGenres(CancellationToken cancellationToken = default);
	Task<Genre> UpdateGenre(GenreDto genreDto, CancellationToken cancellationToken = default);
	Task<Genre> DeleteGenre(string name, CancellationToken cancellationToken = default);
}
