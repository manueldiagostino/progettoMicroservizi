using AuthorsHandler.Shared;
using AuthorsHandler.Repository.Model;

namespace AuthorsHandler.Business.Abstraction {
	public interface IBusiness {

		public Task<bool> CreateAuthor(AuthorDto author, CancellationToken ct = default);
		public Task<Author?> RemoveAuthor(AuthorDto author, CancellationToken ct = default);

		public Task<int?> UpdateAuthor(AuthorDto oldAuthor, AuthorDto newAuthor, CancellationToken ct = default);
		public Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default);

		public Task<ExternalLink?> InsertExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct = default);

		public Task<int?> UpdateExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct = default);
		public Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
		public Task<ExternalLink?> RemoveExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct = default);
	}
}