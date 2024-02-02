using UsersHandler.Repository.Model;
using SixLabors.ImageSharp;
using UsersHandler.Shared;
using GlobalUtility.Kafka.Model;

namespace UsersHandler.Repository.Abstraction;

public interface IRepository {
	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	// Operazioni CRUD per User
	public Task CreateUser(UserDto userDto, CancellationToken cancellationToken = default);
	public Task<User> GetUserFromId(int userId, CancellationToken cancellationToken = default);
	public Task<User> GetUserFromUsername(string username, CancellationToken cancellationToken = default);
	public Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default);
	public Task<User> UpdateUsername(int userId, string username, CancellationToken cancellationToken = default);
	public Task<User> UpdatePassword(int userId, string password, CancellationToken cancellationToken = default);
	public Task<User> DeleteUser(int userId, CancellationToken cancellationToken = default);
	public Task<ICollection<User>> GetAllUsers(CancellationToken cancellationToken = default);

	// Operazioni CRUD per ProPicture
	public Task<string?> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default);
	public Task<string?> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default);
	public Task<User> UploadProfilePictureFromId(int userId, string picturePath, CancellationToken cancellationToken = default);
	public Task<string> DeleteImage(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Bio
	public Task<User> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default);
	public Task<User> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default);
	public Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default);
	public Task<User> DeleteBioFromId(int userId, CancellationToken cancellationToken = default);

	// TransactionalOutbox per Kafka
	public Task InsertTransactionalOutbox(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default);
	public Task DeleteTransactionalOutboxFromId(int id, CancellationToken cancellationToken = default);
	public Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutboxes(CancellationToken ct = default);
}
