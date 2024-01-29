using AuthorsHandler.Shared;
using AuthorsHandler.Repository.Model;

namespace AuthorsHandler.Business.Abstraction {
	public interface IBusiness {

		public Task<bool> CreateAuthor(AuthorDto author, CancellationToken ct = default);
		public Task<Author> RemoveAuthor(AuthorDto author, CancellationToken ct = default);
		public Task<Author> UpdateAuthor(AuthorDto oldAuthor, AuthorDto newAuthor, CancellationToken ct = default);
		public Task<int> GetAuthorIdFromName(string name, string surname, CancellationToken ct = default);
		public Task<Author> GetAuthorFromId(int authorId, CancellationToken ct = default);
		public Task<ICollection<Author>> GetAllAuthors(CancellationToken ct = default);


		public Task<ExternalLink> InsertExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct = default);
		public Task<ExternalLink> UpdateExternalLinkForAuthor(AuthorDto authorDto, int linkId, string newUrl, CancellationToken ct = default);
		public Task<ICollection<string>> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct = default);
		public Task<ExternalLink> RemoveExternalLinkForAuthor(AuthorDto authorDto, int linkId, CancellationToken ct = default);
	}
}