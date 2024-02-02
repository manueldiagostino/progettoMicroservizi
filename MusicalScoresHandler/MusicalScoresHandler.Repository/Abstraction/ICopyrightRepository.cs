using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicalScoresHandler.Repository.Abstraction {
	public interface ICopyrightRepository {
		// Operazioni CRUD per Copyright
		public Task CreateCopyright(CopyrightDto copyrightDto, CancellationToken cancellationToken = default);
		public Task<Copyright> GetCopyrightByName(string name, CancellationToken cancellationToken = default);
		public Task<List<Copyright>> GetAllCopyrights(CancellationToken cancellationToken = default);
		public Task<Copyright> UpdateCopyright(string oldName, string newName, CancellationToken cancellationToken = default);
		public Task<Copyright> DeleteCopyright(string name, CancellationToken cancellationToken = default);
	}
}
