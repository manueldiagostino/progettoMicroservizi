using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using GlobalUtility.Kafka.Model;
using GlobalUtility.Manager.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthorsHandler.Repository {
	public class Repository : IRepository {
		protected AuthorsHandlerDbContext _dbContext;

		public Repository(AuthorsHandlerDbContext authorsHandlerDbContext) {
			_dbContext = authorsHandlerDbContext;
		}

		private IQueryable<Author> GetQueryable(int id) {
			return _dbContext.Authors
				.Where(x => x.id == id);
		}

		private IQueryable<Author> GetQueryable(string name, string surname) {
			return _dbContext.Authors.Where(x =>
				x.name.ToLower().Equals(name.ToLower()) &&
				x.surname.ToLower().Equals(surname.ToLower())
			);
		}

		private async Task<Author> GetUnique(int id, CancellationToken cancellationToken = default) {
			var queryable = GetQueryable(id);

			List<Author> authorList = await queryable.ToListAsync(cancellationToken: cancellationToken);
			if (authorList.Count != 1)
				throw new RepositoryException($"Found <{authorList.Count}> authors for id <{id}>");

			return authorList[0];
		}

		private async Task<Author> GetUnique(string name, string surname, CancellationToken cancellationToken = default) {
			var queryable = GetQueryable(name, surname);

			List<Author> authorList = await queryable.ToListAsync(cancellationToken: cancellationToken);
			if (authorList.Count != 1)
				throw new RepositoryException($"Found <{authorList.Count}> author for <{surname} {name}>");

			return authorList[0];
		}

		public int SaveChanges() {
			int result = -1;
			try {
				result = _dbContext.SaveChanges();
			} catch (Exception e) {
				System.Console.WriteLine(e.Message);
				result = 0;
			}

			return result;
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			return await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task CreateAuthor(string name, string surname, CancellationToken ct) {
			Author author = new() {
				name = name,
				surname = surname
			};

			await _dbContext.AddAsync(author, ct);
		}

		public async Task<int> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
			return (await GetUnique(name, surname, ct)).id;
		}


		public async Task<ICollection<string>> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
			var id = await GetUnique(name, surname, ct);

			var res = await _dbContext.ExternalLinks
				.Where(l => l.authorId.Equals(id))
				.ToListAsync(ct);

			List<string> urls = [];
			foreach (ExternalLink el in res)
				urls.Add(el.url);

			return urls;
		}

		public async Task<Author> RemoveAuthor(string name, string surname, CancellationToken ct = default) {
			var author = await GetUnique(name, surname, ct);
			_dbContext.Authors.Remove(author);

			return author;
		}

		public async Task<Author> UpdateAuthor(string name, string surname, string newName, string newSurname, CancellationToken ct = default) {
			var queryable = GetQueryable(name, surname);
			var author = await GetUnique(name, surname, ct);

			await queryable.ExecuteUpdateAsync(a => a
				.SetProperty(n => n.name, newName)
				.SetProperty(s => s.surname, newSurname)
			, ct);

			return author;
		}

		public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxes(CancellationToken ct = default) {
			return await _dbContext.TransactionalOutboxes.ToListAsync(cancellationToken: ct);
		}

		public async Task InsertTransactionalOutbox(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default) {
			await _dbContext.AddAsync(transactionalOutbox, cancellationToken);
		}
		public async Task DeleteTransactionalOutboxFromId(int id, CancellationToken cancellationToken = default) {
			var res = _dbContext.TransactionalOutboxes.Where(x => x.id == id).ToList();
			if (res.Count <= 0)
				throw new RepositoryException("res.Count() <= 0");

			_dbContext.Remove(res[0]);
			await Task.CompletedTask;
		}

		public static string SanitizeUrl(string originalUrl) {
			if (string.IsNullOrEmpty(originalUrl))
				throw new ArgumentException("IsNullOrEmpty(originalUrl)");

			try {
				// Analizza l'URL originale
				Uri uri = new Uri(originalUrl);

				// Crea un nuovo UriBuilder per costruire l'URL sanificato
				UriBuilder uriBuilder = new UriBuilder(uri);

				// Rimuove il frammento (parte dopo #) se presente
				uriBuilder.Fragment = string.Empty;

				// Rimuove le query string se presente
				uriBuilder.Query = string.Empty;

				// Codifica i caratteri non validi nell'URL
				uriBuilder.Path = Uri.EscapeDataString(uriBuilder.Path);

				// Costruisce e restituisce l'URL sanificato come stringa
				return uriBuilder.Uri.ToString();
			} catch (UriFormatException) {
				return string.Empty;
			}
		}

		public async Task<ExternalLink> InsertExternalLinkForAuthor(string name, string surname, string url, CancellationToken ct) {
			ExternalLink target = new() {
				authorId = (await GetUnique(name, surname, ct)).id,
				url = Repository.SanitizeUrl(url)
			};

			await _dbContext.AddAsync(target, ct);
			return target;
		}

		public async Task<ExternalLink> UpdateExternalLinkForAuthor(string name, string surname, int linkId, string newUrl, CancellationToken ct = default) {
			int id = (await GetUnique(name, surname, ct)).id;

			await _dbContext.ExternalLinks
				.Where(e => e.authorId == id && e.id == linkId)
				.ExecuteUpdateAsync(e =>
					e.SetProperty(x => x.url, newUrl)
			, ct);

			var res = await _dbContext.ExternalLinks
				.Where(e => e.authorId == id)
				.ToListAsync(ct);

			if (res.Count != 1)
				throw new RepositoryException($"Found <{res.Count}> links for author <{surname} {name}>");

			return res[0];
		}

		public async Task<ExternalLink> RemoveExternalLinkForAuthor(string name, string surname, int linkId, CancellationToken ct = default) {
			int id = (await GetUnique(name, surname, ct)).id;

			List<ExternalLink>? res = await _dbContext.ExternalLinks
				.Where(e => e.authorId == id && e.id == linkId)
				.ToListAsync(ct);

			if (res.Count != 1)
				throw new RepositoryException($"Found <{res.Count}> links for author <{surname} {name}>");

			_dbContext.Remove(res[0]);
			return res[0];
		}

		public async Task<Author> GetAuthorFromId(int authorId, CancellationToken ct) {
			return await GetUnique(authorId, ct);
		}

		public async Task<ICollection<Author>> GetAllAuthors(CancellationToken ct = default) {
			return await _dbContext.Authors.ToListAsync(ct);
		}
	}
}