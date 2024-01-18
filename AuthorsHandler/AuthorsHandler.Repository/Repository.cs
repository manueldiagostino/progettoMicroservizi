using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthorsHandler.Repository
{
    public class Repository : IRepository {
		protected AuthorsHandlerDbContext authorsHandlerDbContext_;

		public Repository(AuthorsHandlerDbContext authorsHandlerDbContext) {
			authorsHandlerDbContext_ = authorsHandlerDbContext;
		}

		public int SaveChanges() {
			int result = -1;
			try {
				result = authorsHandlerDbContext_.SaveChanges();
			} catch (Exception e) {
				System.Console.WriteLine(e.Message);
				result=0;
			}

			return result;
		}

		public async Task SaveChangesAsync() {
			await authorsHandlerDbContext_.SaveChangesAsync();
		}

        public async Task CreateAuthor(string name, string surname, CancellationToken ct) {
            Author author = new()
            {
                name = name,
                surname = surname
            };

            await authorsHandlerDbContext_.AddAsync(author, ct);
        }

		public async Task<int?> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
			var res = await authorsHandlerDbContext_.Authors
				.Where(n => n.name.ToLower().Equals(name.ToLower()))
				.ToListAsync(ct);
				
			if (res.Count != 1)
				return null;
			
			return res[0].id;
		}

		public async Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
			var id = await GetAuthorIdFromName(name, surname, ct);
			if (id == null)
				return null;

			var res = await authorsHandlerDbContext_.ExternalLinks
				.Where(l => l.authorId.Equals(id))
				.ToListAsync(ct);
			
			List<string> urls = [];
			foreach (ExternalLink el in res)
                urls.Add(el.url);

			return urls;
		}
    }
}