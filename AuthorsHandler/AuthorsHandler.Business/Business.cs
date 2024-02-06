using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Repository.Abstraction;
using AuthorsHandler.Repository.Model;
using AuthorsHandler.Shared;
using Microsoft.Extensions.Logging;

using AutoMapper;
using AuthorsHandler.Business.Kafka;
using GlobalUtility.Kafka.Factory;
using System.Text.Json;
// using GlobalUtility.Kafka.Model;

namespace AuthorsHandler.Business {
	public class Business : IBusiness {
		private readonly IRepository _repository;
		private readonly ILogger _logger;

		// private readonly IMapper mapper_;

		public Business(IRepository repository, ILogger<Business> logger) {
			_repository = repository;
			_logger = logger;
		}

		public async Task<bool> CreateAuthor(AuthorDto authorDto, CancellationToken ct) {
			int createdCount = 0;

			try {
				await _repository.CreateAuthor(authorDto.name, authorDto.surname, ct);
				createdCount = await _repository.SaveChangesAsync(ct);

				if (createdCount != 1)
					return false;

				var newAuthorTransactional = new AuthorTransactionalDto() {
					id = await _repository.GetAuthorIdFromName(authorDto.name, authorDto.surname, ct),
					name = authorDto.name,
					surname = authorDto.surname
				};

				await _repository.InsertTransactionalOutbox(TransactionalOutboxFactory.CreateInsert<AuthorTransactionalDto>(newAuthorTransactional, KafkaTopicsOutput.Authors), ct);
				await _repository.SaveChangesAsync(ct);
				_logger.LogInformation("Author " + authorDto.name + " " + authorDto.surname + " added");
			} catch (Exception) {
				createdCount = 0;
			}

			return createdCount == 1;
		}

		public async Task<Author> RemoveAuthor(AuthorDto authorDto, CancellationToken ct = default) {
			var res = await _repository.RemoveAuthor(authorDto.name, authorDto.surname, ct);

			var newAuthorTransactional = new AuthorTransactionalDto() {
				id = res.id,
				name = res.name,
				surname = res.surname
			};

			await _repository.InsertTransactionalOutbox(TransactionalOutboxFactory.CreateDelete<AuthorTransactionalDto>(newAuthorTransactional, KafkaTopicsOutput.Authors), ct);
			await _repository.SaveChangesAsync(ct);

			_logger.LogInformation($"Author {JsonSerializer.Serialize(res)} deleted");

			return res;
		}

		public async Task<int> GetAuthorIdFromName(string name, string surname, CancellationToken ct) {
			return await _repository.GetAuthorIdFromName(name, surname, ct); ;
		}

		public async Task<ICollection<string>> GetExternalLinksForAuthor(string name, string surname, CancellationToken ct) {
			return await _repository.GetExternalLinksForAuthor(name, surname, ct); ;
		}

		public async Task<Author> UpdateAuthor(AuthorDto oldAuthor, AuthorDto newAuthor, CancellationToken ct = default) {
			var res = await _repository.UpdateAuthor(oldAuthor.name, oldAuthor.surname, newAuthor.name, newAuthor.surname, ct);

			var newAuthorTransactional = new AuthorTransactionalDto() {
				id = res.id,
				name = newAuthor.name,
				surname = newAuthor.surname
			};

			await _repository.InsertTransactionalOutbox(TransactionalOutboxFactory.CreateUpdate<AuthorTransactionalDto>(newAuthorTransactional, KafkaTopicsOutput.Authors), ct);
			await _repository.SaveChangesAsync(ct);

			_logger.LogInformation($"Author {JsonSerializer.Serialize(oldAuthor)} updated with {JsonSerializer.Serialize(newAuthor)}");
			return res;
		}

		public async Task<ExternalLink> InsertExternalLinkForAuthor(AuthorDto authorDto, string url, CancellationToken ct) {
			var res = await _repository.InsertExternalLinkForAuthor(authorDto.name, authorDto.surname, url, ct);
			await _repository.SaveChangesAsync(ct);

			_logger.LogInformation($"Link {JsonSerializer.Serialize(res)} added");
			return res;
		}

		public async Task<ExternalLink> UpdateExternalLinkForAuthor(AuthorDto authorDto, int linkId, string newUrl, CancellationToken ct = default) {
			var res = await _repository.UpdateExternalLinkForAuthor(authorDto.name, authorDto.surname, linkId, newUrl, ct);
			await _repository.SaveChangesAsync(ct);

			_logger.LogInformation($"Link {JsonSerializer.Serialize(res)} updated");
			return res;
		}

		public async Task<ExternalLink> RemoveExternalLinkForAuthor(AuthorDto authorDto, int linkId, CancellationToken ct = default) {
			var res = await _repository.RemoveExternalLinkForAuthor(authorDto.name, authorDto.surname, linkId, ct);
			await _repository.SaveChangesAsync(ct);

			_logger.LogInformation($"Link {JsonSerializer.Serialize(res)} removed");
			return res;
		}

		public async Task<Author> GetAuthorFromId(int authorId, CancellationToken ct = default) {
			return await _repository.GetAuthorFromId(authorId, ct);
		}

		public async Task<ICollection<Author>> GetAllAuthors(CancellationToken ct = default) {
			return await _repository.GetAllAuthors(ct);
		}
	}
}