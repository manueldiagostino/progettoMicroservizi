using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using AuthorsHandler.Shared;
using Microsoft.Extensions.Logging;

namespace AuthorsHandler.Business
{
	public class Business : IBusiness {
		private readonly IRepository repository_;
		private readonly ILogger logger_;

		public Business(IRepository repository, ILogger<Business> logger) {
			repository_ = repository;
			logger_ = logger;
		}

		public async Task<bool> CreateAuthor(AuthorDto author, CancellationToken ct) {
			await repository_.CreateAuthor(author.name, author.surname, ct);
			var created = await repository_.SaveChangesAsync(ct);

			return created==1;
		}

        public async Task<Author?> RemoveAuthor(AuthorDto author, CancellationToken ct = default) {
			var res = await repository_.RemoveAuthor(author.name, author.surname, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
        }

        public async Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
			return await repository_.GetAuthorIdFromName(name, surname, ct);;
        }

		public async Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
			return await repository_.GetExternalLinksForAuthor(name, surname, ct);;
        }

        public async Task<int?> UpdateAuthor(AuthorDto oldAuthor, AuthorDto newAuthor, CancellationToken ct = default) {
            var res = await repository_.UpdateAuthor(oldAuthor.name, oldAuthor.surname, newAuthor.name, newAuthor.surname, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
        }
    }
}