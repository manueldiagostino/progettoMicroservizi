using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Repository.Abstraction;
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
			var created = repository_.SaveChanges();

			return created==1;
		}

        public async Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
            var res = await repository_.GetAuthorIdFromName(name, surname, ct);
			return res;
        }

		public async Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
            var res = await repository_.GetExternalLinksForAuthor(name, surname, ct);
			return res;
        }
    }
}