using UsersHandler.Repository.Model;
using SixLabors.ImageSharp;
using UsersHandler.Shared;

namespace UsersHandler.Repository.Abstraction;

public interface IRepository {
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	// Operazioni CRUD per User
	Task CreateUser(UserDto userDto, CancellationToken cancellationToken = default);
	Task<User> GetUserFromId(int userId, CancellationToken cancellationToken = default);
	Task<User> GetUserFromUsername(string username, CancellationToken cancellationToken = default);
	Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default);
	Task<User> UpdateUsername(int userId, string username, CancellationToken cancellationToken = default);
	Task<User> DeleteUser(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per ProPicture
	Task<string?> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default);
	Task<string?> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default);
	Task<User> UploadProfilePictureFromId(int userId, string picturePath, CancellationToken cancellationToken = default);
	Task<User> DeleteImage(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Bio
	public Task<User> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default);
	Task<User> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default); 
	Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default); 
	Task<User> DeleteBioFromId(int userId, CancellationToken cancellationToken = default); 
}
