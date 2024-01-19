using AuthorsHandler.Repository.Model;

namespace AuthorsHandler.Repository.Abstraction
{
	public interface IRepository {

		public int SaveChanges();
		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		public Task CreateAuthor(string name, string surname, CancellationToken ct = default);

		public Task<int?> UpdateAuthor(string name, string surname, string newName, string newSurname, CancellationToken ct = default);
		public Task<Author?> RemoveAuthor(string name, string surname, CancellationToken ct = default);
		public Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default); 
		public Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
	}
}