using SixLabors.ImageSharp;

namespace UsersHandler.Shared;

public class UserDto {
	public string Username { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public string Password { get; set; }
}
