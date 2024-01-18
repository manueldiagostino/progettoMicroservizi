using AuthorsHandler.Shared;

namespace AuthorsHandler.Business.Abstraction
{
	public interface IBusiness {

		public Task<bool> CreateAuthor(AuthorDto author, CancellationToken ct = default);
		public Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default);
		public Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
	}
}