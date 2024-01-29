using Microsoft.AspNetCore.Mvc;
using UsersHandler.Business.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace UsersHandler.Api.Controllers;

using UserOutType = UserTransactionalDto;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase {
	private readonly IBusiness _business;
	private readonly ILogger _logger;

	public UsersController(IBusiness business, ILogger<UsersController> logger) {
		_business = business;
		_logger = logger;
	}

	private bool ValidateFile(IFormFile file) {

		var fileExtension = Path.GetExtension(file.FileName);
		var allowedExtensions = new[] { ".png", ".jpeg", ".jpg" };

		if (!allowedExtensions.Contains(fileExtension.ToLower())) {
			return false;
		}

		return true;
	}

	[HttpPost(Name = "CreateUser")]
	public async Task<ActionResult> CreateUser([FromQuery] UserDto userDto) {
		try {

			int? res = await _business.CreateUser(userDto);
			return (res == 1) ? Ok($"User <{userDto.Username}> added") : BadRequest("No users added");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpGet(Name = "GetIdFromUsername")]
	public async Task<ActionResult> GetIdFromUsername([FromQuery] string username) {
		try {

			int id = await _business.GetIdFromUsername(username);
			return Ok($"User <{username}> has id <{id}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpGet(Name = "GetUsernameFromId")]
	public async Task<ActionResult> GetUsernameFromId([FromQuery] int id) {
		try {

			UserOutType user = await _business.GetUserFromId(id);
			return Ok($"User <{user.UserId}> has username <{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpGet(Name = "GetUserFromId")]
	public async Task<ActionResult> GetUserFromId([FromQuery] int id) {
		try {

			UserOutType user = await _business.GetUserFromId(id);
			return Ok($"{JsonSerializer.Serialize(user)}");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpPut(Name = "UpdateUsername")]
	public async Task<ActionResult> UpdateUsername([FromQuery] int id, string username) {
		try {

			UserOutType user = await _business.UpdateUsername(id, username);
			return Ok($"Updated user <{user.UserId},{user.Username}> with username <{username}> ");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpDelete(Name = "DeleteUser")]
	public async Task<ActionResult> DeleteUser([FromQuery] int id) {
		try {

			UserOutType user = await _business.DeleteUser(id);
			return Ok($"User <{user.UserId}, {user.Username}> deleted");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpGet(Name = "VerifyPassword")]
	public async Task<ActionResult> VerifyPassword([FromQuery] int userId, string password) {
		try {

			bool verified = await _business.VerifyPassword(userId, password);
			return Ok($"{verified}");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpPut(Name = "UpdatePassword")]
	public async Task<ActionResult> UpdatePassword([FromQuery] int userId, string oldPassword, string newPassword) {
		try {

			UserOutType user = await _business.UpdatePassword(userId, oldPassword, newPassword);
			return Ok($"Updated user <{user.UserId},{user.Username}> with the new password");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpPost(Name = "UploadProfilePictureFromId")]
	public async Task<IActionResult> UploadProfilePictureFromId([FromQuery] int userId, [FromForm] IFormFile file) {
		if (file == null || file.Length == 0) {
			return BadRequest("File not valid");
		}

		if (!ValidateFile(file)) {
			return BadRequest($"File extension not allowed");
		}

		try {

			UserOutType user = await _business.UploadProfilePictureFromId(userId, file);
			return (user != null) ? Ok($"Uploaded image for {user.Username}") : BadRequest("An error occured");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpPost(Name = "UploadProfilePictureFromUsername")]
	public async Task<IActionResult> UploadProfilePictureFromUsername([FromQuery, Required] string username, [FromForm, Required] IFormFile file) {
		if (file == null || file.Length == 0) {
			return BadRequest("File not valid");
		}

		if (!ValidateFile(file)) {
			return BadRequest($"File extension not allowed");
		}
		try {

			int id = await _business.GetIdFromUsername(username);
			UserOutType user = await _business.UploadProfilePictureFromId(id, file);

			return Ok($"Uploaded image for <{user.UserId},{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	private static string GetContentType(string filePath) {
		try {
			using var imageStream = System.IO.File.OpenRead(filePath);
			var format = Image.DetectFormat(imageStream);

			return format?.DefaultMimeType ?? "application/octet-stream";
		} catch (Exception) {
			return "application/octet-stream";
		}
	}

	[HttpGet(Name = "DownloadProfilePicture")]
	public async Task<IActionResult> DownloadProfilePicture(string username) {
		try {

			string? imagePath = await _business.GetProfilePictureFromUsername(username);
			if (string.IsNullOrEmpty(imagePath))
				return NotFound($"No image for user <{username}>");

			if (!System.IO.File.Exists(imagePath)) {
				return NotFound($"No image for path <{imagePath}>");
			}

			string contentType = GetContentType(imagePath);

			return PhysicalFile(imagePath, contentType, Path.GetFileName(imagePath));

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpDelete(Name = "DeleteImage")]
	public async Task<ActionResult> DeleteImage([FromQuery] int id) {
		try {

			UserOutType user = await _business.DeleteImage(id);
			return Ok($"User <{user.UserId}, {user.Username}> deleted");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

}
