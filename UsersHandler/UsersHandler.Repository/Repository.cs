using SixLabors.ImageSharp;
using UsersHandler.Repository.Abstraction;
using UsersHandler.Repository.Model;

namespace UsersHandler.Repository;

public class Repository : IRepository {
	public Task<int?> GetIdFromUsername(string username, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<Image> GetImageFromId(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<Image> GetImageFromUsername(string username, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<User?> GetUserFromId(int id, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<User?> GetUserFromUsername(string username, CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		throw new NotImplementedException();
	}
}
