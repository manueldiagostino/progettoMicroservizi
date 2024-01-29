using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using AuthorsHandler.Shared;
using Microsoft.Extensions.Logging;

using AutoMapper;
using AuthorsHandler.Business.Kafka;
using GlobalUtility.Kafka.Factory;
// using GlobalUtility.Kafka.Model;

namespace AuthorsHandler.Business {
	public class Business : IBusiness {
		private readonly IRepository repository_;
		private readonly ILogger logger_;

		// private readonly IMapper mapper_;

		public Business(IRepository repository, ILogger<Business> logger) {
			repository_ = repository;
			logger_ = logger;
		}

		public async Task<bool> CreateAuthor(AuthorDto authorDto, CancellationToken ct) {
			int createdCount = 0;

			try {
				await repository_.CreateAuthor(authorDto.name, authorDto.surname, ct);
				createdCount = await repository_.SaveChangesAsync(ct);

				if (createdCount != 1)
					return false;

				var newAuthor = new AuthorTransactionalDto() {
					id = await repository_.GetAuthorIdFromName(authorDto.name, authorDto.surname, ct),
					name = authorDto.name,
					surname = authorDto.surname
				};

				await repository_.InsertTransactionalOutbox(TransactionalOutboxFactory.CreateInsert<AuthorTransactionalDto>(newAuthor, KafkaTopicsOutput.Authors), ct);
				await repository_.SaveChangesAsync(ct);
			} catch (Exception) {
				createdCount = 0;
			}

			return createdCount == 1;
		}

		public async Task<Author> RemoveAuthor(AuthorDto author, CancellationToken ct = default) {
			var res = await repository_.RemoveAuthor(author.name, author.surname, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
		}

		public async Task<int> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
			return await repository_.GetAuthorIdFromName(name, surname, ct); ;
		}

		public async Task<ICollection<string>?> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
			return await repository_.GetExternalLinksForAuthor(name, surname, ct); ;
		}

		public async Task<Author> UpdateAuthor(AuthorDto oldAuthor, AuthorDto newAuthor, CancellationToken ct = default) {
			var res = await repository_.UpdateAuthor(oldAuthor.name, oldAuthor.surname, newAuthor.name, newAuthor.surname, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
		}

		public async Task<ExternalLink> InsertExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct) {
			var res = await repository_.InsertExternalLinkForAuthor(authorDto.name, authorDto.surname, url, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
		}

		public async Task<ExternalLink> UpdateExternalLinkForAuthor(AuthorDto authorDto, int linkId, string newUrl, CancellationToken ct = default) {
			var res = await repository_.UpdateExternalLinkForAuthor(authorDto.name, authorDto.surname,linkId, newUrl, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
		}

		public async Task<ExternalLink> RemoveExternalLinkForAuthor(AuthorDto authorDto, int linkId, CancellationToken ct = default) {
			var res = await repository_.RemoveExternalLinkForAuthor(authorDto.name, authorDto.surname, linkId, ct);
			await repository_.SaveChangesAsync(ct);

			return res;
		}

		public async Task<Author> GetAuthorFromId(int authorId, CancellationToken ct = default) {
			return await repository_.GetAuthorFromId(authorId, ct);
		}
	}
}