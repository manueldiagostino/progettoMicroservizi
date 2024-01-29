using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IAuthorKafkaRepository {
	public Task SaveChangesAsync(CancellationToken cancellationToken = default);
	public Task InsertAuthor(AuthorKafka authorKafka, CancellationToken cancellationToken = default);
	public Task<AuthorKafka> UpdateAuthor(AuthorKafka authorKafka, CancellationToken cancellationToken = default);
	public Task<AuthorKafka> DeleteAuthor(int authorId, CancellationToken cancellationToken = default);

	public Task<bool> CheckAuthorId(int authorId, CancellationToken cancellationToken = default);
}
