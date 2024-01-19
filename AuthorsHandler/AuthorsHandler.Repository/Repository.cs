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

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			return await authorsHandlerDbContext_.SaveChangesAsync(cancellationToken);
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
			
			return (res.Count != 1) ? null : res[0].id;
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

		private async Task<Author?> GetAuthorFromId(int id, CancellationToken ct) {
			var res = await authorsHandlerDbContext_.Authors
				.Where(a => a.id == id)
				.ToListAsync(ct);

			return (res.Count == 0) ? null : res[0];
		}

        public async Task<Author?> RemoveAuthor(string name, string surname, CancellationToken ct = default) {
			var id_res = await GetAuthorIdFromName(name, surname, ct);
			if (id_res == null)
				return null;
			
			int id_not_null = (int)(id_res);

			Author? author = await GetAuthorFromId(id_not_null, ct);
			if (author == null)
				return null;
		
            authorsHandlerDbContext_.Authors.Remove(author);
			return author;
        }

        public async Task<int?> UpdateAuthor(string name, string surname, string newName, string newSurname, CancellationToken ct = default) {
			return await authorsHandlerDbContext_.Authors
                .Where(a => a.name.Equals(name) && a.surname.Equals(surname))
				.ExecuteUpdateAsync(a => a
					.SetProperty(n => n.name, newName)
					.SetProperty(s => s.surname, newSurname)
				, ct);
        }
    }
}