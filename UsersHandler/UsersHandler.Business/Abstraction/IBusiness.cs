using SixLabors.ImageSharp;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;
using Microsoft.AspNetCore.Http;

namespace UsersHandler.Business.Abstraction;

using UserOutType = UserTransactionalDto;
public interface IBusiness {

	// Operazioni CRUD per User
	public Task<int> CreateUser(UserDto userDto, CancellationToken cancellationToken = default);
	Task<UserOutType> GetUserFromId(int userId, CancellationToken cancellationToken = default);
	Task<UserOutType> GetUserFromUsername(string username, CancellationToken cancellationToken = default);
	Task<int> GetIdFromUsername(string username, CancellationToken cancellationToken = default);
	Task<UserOutType> UpdateUsername(int userId, string username, CancellationToken cancellationToken = default);
	Task<UserOutType> DeleteUser(int userId, CancellationToken cancellationToken = default);

	Task<bool> VerifyPassword(int userId, string password, CancellationToken cancellationToken = default);
	Task<UserOutType> UpdatePassword(int userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Image
	public Task<UserOutType> UploadProfilePictureFromId(int userId, IFormFile profilePicture, CancellationToken cancellationToken = default);
	Task<string> GetProfilePictureFromId(int userId, CancellationToken cancellationToken = default);
	Task<string> GetProfilePictureFromUsername(string username, CancellationToken cancellationToken = default);
	Task<UserOutType> DeleteImage(int userId, CancellationToken cancellationToken = default);

	// Operazioni CRUD per Bio
	public Task<UserOutType> CreateBioFromId(BioDto bioDto, CancellationToken cancellationToken = default);
	Task<UserOutType> SetBioFromId(BioDto bioDto, CancellationToken cancellationToken = default); 
	Task<string?> GetBioFromId(int userId, CancellationToken cancellationToken = default); 
	Task<UserOutType> DeleteBioFromId(int userId, CancellationToken cancellationToken = default); 
}
