using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Repository.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using Microsoft.EntityFrameworkCore;
using GlobalUtility.Manager.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicalScoresHandler.Repository.Repository {
	public class CopyrightRepository : ICopyrightRepository {
		protected readonly MusicalScoresHandlerDbContext _dbContext;
		private readonly ILogger _logger;

		public CopyrightRepository(MusicalScoresHandlerDbContext musicalScoresHandlerDbContext, ILogger<Repository> logger) {
			_dbContext = musicalScoresHandlerDbContext;
			_logger = logger;
		}

		public async Task CreateCopyright(CopyrightDto copyrightDto, CancellationToken cancellationToken = default) {
			Copyright copyright = new() {
				Name = copyrightDto.Name
			};

			await _dbContext.AddAsync(copyright, cancellationToken);
		}

		private IQueryable<Copyright> GetQueryable(string name) {
			return _dbContext.Copyrights
				.Where(x => x.Name.Equals(name));
		}

		private IQueryable<Copyright> GetQueryable(int id) {
			return _dbContext.Copyrights
				.Where(x => x.Id == id);
		}

		private async Task<Copyright> GetUnique(string name, CancellationToken cancellationToken = default) {
			var queryable = _dbContext.Copyrights
				.Where(x => x.Name.Equals(name));

			List<Copyright> copyrightList = await queryable.ToListAsync(cancellationToken: cancellationToken);
			if (copyrightList.Count != 1)
				throw new RepositoryException($"Found <{copyrightList.Count}> copyrights for name <{name}>");

			return copyrightList[0];
		}

		private async Task<Copyright> GetUnique(int id, CancellationToken cancellationToken = default) {
			var queryable = _dbContext.Copyrights
				.Where(x => x.Id == id);

			List<Copyright> copyrightList = await queryable.ToListAsync(cancellationToken: cancellationToken);
			if (copyrightList.Count != 1)
				throw new RepositoryException($"Found <{copyrightList.Count}> copyrights for id <{id}>");

			return copyrightList[0];
		}

		public async Task<Copyright> DeleteCopyright(string name, CancellationToken cancellationToken = default) {
			Copyright copyright = await GetUnique(name, cancellationToken);

			_dbContext.Remove(copyright);
			return copyright;
		}

		public async Task<List<Copyright>> GetAllCopyrights(CancellationToken cancellationToken = default) {
			return await _dbContext.Copyrights.ToListAsync(cancellationToken);
		}

		public async Task<Copyright> GetCopyrightByName(string name, CancellationToken cancellationToken = default) {
			return await GetUnique(name, cancellationToken);
		}

		public async Task<Copyright> UpdateCopyright(CopyrightDto copyrightDto, CancellationToken cancellationToken = default) {
			var queryable = GetQueryable(copyrightDto.Name);

			List<Copyright> copyrightList = await queryable.ToListAsync(cancellationToken: cancellationToken);
			if (copyrightList.Count != 1)
				throw new RepositoryException($"Found <{copyrightList.Count}> copyrights for name <{copyrightDto.Name}>");

			await queryable.ExecuteUpdateAsync(x => x
				.SetProperty(x => x.Name, copyrightDto.Name)
			, cancellationToken: cancellationToken);

			return copyrightList[0];
		}

	}
}
