using AuthorsHandler.Repository.Model;
using GlobalUtility.Kafka.Model;

namespace AuthorsHandler.Repository.Abstraction {
	public interface IRepository {

		public int SaveChanges();
		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		public Task CreateAuthor(string name, string surname, CancellationToken ct = default);
		public Task<int> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default);
		public Task<Author> GetAuthorFromId(int authorId, CancellationToken ct = default);

		public Task<Author> UpdateAuthor(string name, string surname, string newName, string newSurname, CancellationToken ct = default);
		public Task<Author> RemoveAuthor(string name, string surname, CancellationToken ct = default);
		public Task<ExternalLink> InsertExternalLinkForAuthor(string name, string surname, string url, CancellationToken ct = default);
		public Task<ICollection<string>> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
		public Task<ExternalLink> UpdateExternalLinkForAuthor(string name, string surname, int linkId, string newUrl, CancellationToken ct = default);
		public Task<ExternalLink> RemoveExternalLinkForAuthor(string name, string surname, int linkId, CancellationToken ct = default);


		public Task InsertTransactionalOutbox(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default);
		public Task DeleteTransactionalOutboxFromId(int id, CancellationToken cancellationToken = default);
		public Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxes(CancellationToken ct = default);
	}
}