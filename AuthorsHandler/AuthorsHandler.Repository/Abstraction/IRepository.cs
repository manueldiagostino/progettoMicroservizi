using AuthorsHandler.Repository.Model;

namespace AuthorsHandler.Repository.Abstraction
{
	public interface IRepository {

		public int SaveChanges();
		public Task CreateAuthor(string name, string surname, CancellationToken ct = default);
		public Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default);
		public Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
	}
}