using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;

namespace MusicalScoresHandler.Api.Controllers;

	[ApiController]
	[Route("[controller]/[action]")]
	public class CopyrightsController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger _logger;

		public CopyrightsController(IBusiness business, ILogger<CopyrightsController> logger) {
			_business = business;
			_logger = logger;
		}

		[HttpPost(Name = "CreateCopyright")]
		public async Task<ActionResult> CreateCopyright([FromQuery] CopyrightDto copyrightDto) {
			try {
				await _business.CreateCopyright(copyrightDto);
				return Ok($"Created Copyright <{copyrightDto.Name}>");
			} catch (Exception e) {
				_logger.LogError($"Error creating Copyright: {e}");
				return BadRequest($"Error creating Copyright: {e}");
			}
		}

		[HttpGet(Name = "GetAllCopyrights")]
		public async Task<ActionResult<List<Copyright>>> GetAllCopyrights() {
			try {
				var Copyrights = await _business.GetAllCopyrights();
				return Ok(JsonSerializer.Serialize(Copyrights));
			} catch (Exception e) {
				_logger.LogError($"Error getting all Copyrights: {e}");
				return BadRequest($"Error getting all Copyrights: {e}");
			}
		}

		[HttpGet(Name = "GetCopyrightByName")]
		public async Task<ActionResult<Copyright>> GetCopyrightByName([FromQuery] string name) {
			try {
				var copyright = await _business.GetCopyrightByName(name);

				return Ok(copyright);
			} catch (Exception e) {
				_logger.LogError($"Error getting Copyright by name: {e}");
				return BadRequest($"Error getting Copyright by name: {e}");
			}
		}

		[HttpPut(Name = "UpdateCopyright")]
		public async Task<ActionResult> UpdateCopyright([FromQuery] string oldName, string newName) {
			try {
				var updatedCopyright = await _business.UpdateCopyright(oldName, newName);
				return Ok($"Updated Copyright <{oldName}> to {newName}");
			} catch (Exception e) {
				_logger.LogError($"Error updating Copyright: {e}");
				return BadRequest($"Error updating Copyright: {e}");
			}
		}

		[HttpDelete(Name = "DeleteCopyright")]
		public async Task<ActionResult> DeleteCopyright([FromQuery] string name) {
			try {
				var deletedCopyright = await _business.DeleteCopyright(name);
				return Ok($"Deleted Copyright <{deletedCopyright.Name}>");
			} catch (Exception e) {
				_logger.LogError($"Error deleting Copyright: {e}");
				return BadRequest($"Error deleting Copyright: {e}");
			}
		}
	}