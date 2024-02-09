using Microsoft.AspNetCore.Mvc;
using UsersHandler.Business.Abstraction;
using UsersHandler.Repository.Model;
using UsersHandler.Shared;
using Newtonsoft.Json;

using UserOutType = UsersHandler.Shared.UserTransactionalDto;

namespace UsersHandler.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class BioController : ControllerBase {
	private readonly IBusiness _business;
	private readonly ILogger _logger;

	public BioController(IBusiness business, ILogger<UsersController> logger) {
		_business = business;
		_logger = logger;
	}

	[HttpPost(Name = "CreateBio")]
	public async Task<ActionResult> CreateBio([FromQuery] BioDto bioDto) {
		try {

			UserOutType user = await _business.CreateBioFromId(bioDto);
			return Ok($"Bio added for user <{user.UserId},{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpPut(Name = "SetBioFromId")]
	public async Task<ActionResult> SetBioFromId([FromQuery] BioDto bioDto) {
		try {

			UserOutType user = await _business.SetBioFromId(bioDto);
			return Ok($"Updated user <{user.UserId},{user.Username}> with bio <{JsonConvert.SerializeObject(bioDto)}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpGet(Name = "GetBioFromId")]
	public async Task<ActionResult> GetBioFromId([FromQuery] int userId) {
		try {

			string? bio = await _business.GetBioFromId(userId);
			BioDto bioDto = new BioDto() {
				UserId = userId,
				Text = bio
			};

			return Ok($"{JsonConvert.SerializeObject(bioDto)}");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}

	[HttpDelete(Name = "DeleteBioFromId")]
	public async Task<ActionResult> DeleteBioFromId([FromQuery] int userId) {
		try {

			UserOutType user = await _business.DeleteBioFromId(userId);
			return Ok($"Deleted bio for user <{user.UserId},{user.Username}>");

		} catch (Exception e) {
			return BadRequest($"{e}");
		}
	}
}
