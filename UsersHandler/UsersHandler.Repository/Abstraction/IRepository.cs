using UsersHandler.Repository.Model;
using SixLabors.ImageSharp;

namespace UsersHandler.Repository.Abstraction;

public interface IRepository {
	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	public Task<User?> GetUserFromId(int id, CancellationToken cancellationToken = default);
	public Task<User?> GetUserFromUsername(string username, CancellationToken cancellationToken = default);
	public Task<int?> GetIdFromUsername(string username, CancellationToken cancellationToken = default);

	public Task<Image> GetImageFromId(int id, CancellationToken cancellationToken = default);
	public Task<Image> GetImageFromUsername(string username, CancellationToken cancellationToken = default);
}
