using Microsoft.AspNetCore.Mvc;
using UsersHandler.Business.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace UsersHandler.Api.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase {
	private readonly IBusiness _business;
	private readonly ILogger _logger;

	public UsersController(IBusiness business, ILogger<UsersController> logger) {
		_business = business;
		_logger = logger;
	}

	[HttpPost(Name = "CreateUser")]
	public async Task<ActionResult> CreateUser([FromQuery] UserDto userDto) {
		try {

			int? res = await _business.CreateUser(userDto);
			return (res == 1) ? Ok($"User {userDto.Username} added") : BadRequest("No users added");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpGet(Name = "GetIdFromUsername")]
	public async Task<ActionResult> GetIdFromUsername([FromQuery] string username) {
		try {

			int id = await _business.GetIdFromUsername(username);
			return Ok($"User <{username}> has id <{id}>");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpGet(Name = "GetUsernameFromId")]
	public async Task<ActionResult> GetUsernameFromId([FromQuery] int id) {
		try {

			User user = await _business.GetUserFromId(id);
			return Ok($"User <{user.Id}> has username <{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpPut(Name = "UpdateUsername")]
	public async Task<ActionResult> UpdateUsername([FromQuery] int id, string username) {
		try {

			User user = await _business.UpdateUsername(id, username);
			return Ok($"Updated user <{user.Id},{user.Username}> with username <{username}> ");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpDelete(Name = "DeleteUser")]
	public async Task<ActionResult> DeleteUser([FromQuery] int id) {
		try {

			User user = await _business.DeleteUser(id);
			return Ok($"User <{user.Id}, {user.Username}> deleted");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpPost(Name = "UploadProfilePictureFromId")]
	public async Task<IActionResult> UploadProfilePictureFromId([FromQuery] int userId, [FromForm] IFormFile image) {
		try {

			User user = await _business.UploadProfilePictureFromId(userId, image);
			return (user != null) ? Ok($"Uploaded image for {user.Username}") : BadRequest("An error occured");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpPost(Name = "UploadProfilePictureFromUsername")]
	public async Task<IActionResult> UploadProfilePictureFromUsername([FromQuery, Required] string username, [FromForm, Required] IFormFile image) {
		try {

			int id = await _business.GetIdFromUsername(username);
			User user = await _business.UploadProfilePictureFromId(id, image);

			return Ok($"Uploaded image for <{user.Id},{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
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

	[HttpGet("{resultImageName}, {username}")]
	public async Task<IActionResult> DownloadProfilePicture(string resultImageName, string username) {
		try {

			string? imagePath = await _business.GetProfilePictureFromUsername(username);
			if (string.IsNullOrEmpty(imagePath))
				return NotFound($"No image for user <{username}>");

			if (!System.IO.File.Exists(imagePath)) {
				return NotFound($"no image for path <{imagePath}>");
			}

			string contentType = GetContentType(imagePath);

			return PhysicalFile(imagePath, contentType, resultImageName);

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

	[HttpDelete(Name = "DeleteImage")]
	public async Task<ActionResult> DeleteImage([FromQuery] int id) {
		try {

			User user = await _business.DeleteImage(id);
			return Ok($"User <{user.Id}, {user.Username}> deleted");

		} catch (Exception e) {
			return BadRequest($"{e.Message}");
		}
	}

}
