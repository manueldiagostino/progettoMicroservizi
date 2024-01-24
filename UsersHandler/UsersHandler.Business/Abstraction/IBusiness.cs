using SixLabors.ImageSharp;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;
using Microsoft.AspNetCore.Http;

namespace UsersHandler.Business.Abstraction;

public interface IBusiness {

	// Operazioni CRUD per User
	public Task<int?> CreateUser(UserDto userDto, CancellationToken cancellationToken = default);
	Task<User> GetUserFromId(int userId, CancellationToken cancellationToken = default);
	Task<User> GetUserFromUsername(string username, CancellationToken cancellationToken = default);
	Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default);
	Task<User> UpdateUsername(int userId, string username, CancellationToken cancellationToken = default);
	Task<User> DeleteUser(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Image
	public Task<User> UploadProfilePictureFromId(int userId, IFormFile profilePicture, CancellationToken cancellationToken = default);
	Task<string?> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default);
	Task<string?> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default);
	Task<User> DeleteImage(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Bio
	public Task<User> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default);
	Task<User> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default); 
	Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default); 
	Task<User> DeleteBioFromId(int userId, CancellationToken cancellationToken = default); 
}
