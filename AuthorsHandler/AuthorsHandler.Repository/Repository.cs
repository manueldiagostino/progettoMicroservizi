using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using GlobalUtility.Kafka.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthorsHandler.Repository {
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
				result = 0;
			}

			return result;
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
			return await authorsHandlerDbContext_.SaveChangesAsync(cancellationToken);
		}

		public async Task CreateAuthor(string name, string surname, CancellationToken ct) {
			Author author = new() {
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

		public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxes(CancellationToken ct = default) {
			return await authorsHandlerDbContext_.TransactionalOutboxes.ToListAsync(cancellationToken: ct);
		}

		public async Task InsertTransactionalOutbox(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default) {
			await authorsHandlerDbContext_.AddAsync(transactionalOutbox, cancellationToken);
		}
		public async Task DeleteTransactionalOutboxFromId(int id, CancellationToken cancellationToken = default) {
			var res = authorsHandlerDbContext_.TransactionalOutboxes.Where(x => x.id == id).ToList();
			if (res.Count <= 0)
				throw new ThreadStateException("res.Count() <= 0");

			authorsHandlerDbContext_.Remove(res[0]);
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

				// Rimuovi il frammento (parte dopo #) se presente
				uriBuilder.Fragment = string.Empty;

				// Rimuovi le query string se presente
				uriBuilder.Query = string.Empty;

				// Codifica i caratteri non validi nell'URL
				uriBuilder.Path = Uri.EscapeDataString(uriBuilder.Path);

				// Costruisci e restituisci l'URL sanificato come stringa
				return uriBuilder.Uri.ToString();
			} catch (UriFormatException) {
				// Gestisci il caso in cui l'URL originale non è valido
				return string.Empty;
			}
		}

		public async Task<ExternalLink?> InsertExternalLinkForAuthor(string name, string surname, string url, CancellationToken ct) {
			ExternalLink target = new() {
				authorId = await GetAuthorIdFromName(name, surname, ct) ?? throw new ThreadStateException("GetAuthorIdFromName() returned not valid id"),
				url = Repository.SanitizeUrl(url)
			};

			await authorsHandlerDbContext_.AddAsync(target, ct);
			return target;
		}

		public async Task<int?> UpdateExternalLinkForAuthor(string name, string surname, string url, CancellationToken ct = default) {
			int? id = await GetAuthorIdFromName(name, surname, ct);

			if (id <= 0)
				return null;
			
			return await authorsHandlerDbContext_.ExternalLinks
				.Where(e => e.authorId==id)
				.ExecuteUpdateAsync( e =>
					e.SetProperty(x => x.url, url)
			);
		}

		public async Task<ExternalLink?> RemoveExternalLinkForAuthor(string name, string surname, string url, CancellationToken ct = default) {
			int? id = await GetAuthorIdFromName(name, surname, ct);

			if (id <= 0)
				return null;

			List<ExternalLink>? res = await authorsHandlerDbContext_.ExternalLinks
				.Where(e => e.authorId==id && e.url.Equals(url))
				.ToListAsync();

			if (res.Count <= 0)
				return null;

			authorsHandlerDbContext_.Remove(res[0]);
			return res[0];
		}
	}
}