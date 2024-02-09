using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Shared;
using AuthorsHandler.Repository.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthorsHandler.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class AuthorsController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger _logger;

		public AuthorsController(IBusiness business, ILogger<AuthorsController> logger) {
			_business = business;
			_logger = logger;
		}

		[HttpPut(Name = "CreateAuthor")]
		public async Task<IActionResult> CreateAuthor([FromQuery] AuthorDto authorDto) {

			bool isCreated = await _business.CreateAuthor(authorDto);
			if (!isCreated)
				return BadRequest("The author already exists");

			return Ok("Author " + authorDto.name + " " + authorDto.surname + " added");
		}

		[HttpDelete(Name = "RemoveAuthor")]
		public async Task<IActionResult> DeleteAuthor([FromQuery] AuthorDto authorDto) {
			try {
				Author deletedAuthor = await _business.RemoveAuthor(authorDto);
				return Ok("Author " + deletedAuthor.name + " " + deletedAuthor.surname + " deleted");
			} catch (Exception e) {
				return BadRequest($"The author {JsonSerializer.Serialize(authorDto)} does not exist");
			}
		}

		[HttpPost(Name = "UpdateAuthor")]
		public async Task<IActionResult> UpdateAuthor([FromQuery] UpdateAuthorDto update) {
			AuthorDto oldAuthor = new AuthorDto { name = update.oldName, surname = update.oldSurname };
			AuthorDto newAuthor = new AuthorDto { name = update.newName, surname = update.newSurname };

			try {
				var res = await _business.UpdateAuthor(oldAuthor, newAuthor);
				return Ok($"User updated: {update}");
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpGet(Name = "GetAuthorIdFromName")]
		public async Task<IActionResult> GetAuthorIdFromName(string name, string surname) {
			try {
				int res = await _business.GetAuthorIdFromName(name, surname);
				return Ok($"User id: {res}");
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpGet(Name = "GetAllAuthors")]
		public async Task<IActionResult> GetAllAuthors() {
			try {
				var res = await _business.GetAllAuthors();
				return Ok(res);
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpGet(Name = "GetExternalLinksForAuthor")]
		public async Task<IActionResult> GetExternalLinksForAuthor(string name, string surname) {
			try {
				ICollection<string> res = await _business.GetExternalLinksForAuthor(name, surname);
				return Ok(res);
			} catch (Exception e) {
				return NotFound($"{e}");

			}
		}

		[HttpPut(Name = "InsertExternalLink")]
		public async Task<IActionResult> InsertExternalLink([FromQuery] AuthorDto authorDto, string url) {
			try {
				ExternalLink res = await _business.InsertExternalLinkForAuthor(authorDto, url);
				return Ok($"Inserted link: {res.url}");
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpPost(Name = "UpdateExternalLinkForAuthor")]
		public async Task<IActionResult> UpdateExternalLinkForAuthor([FromQuery] AuthorDto authorDto, int linkId, string newUrl) {
			try {
				ExternalLink res = await _business.UpdateExternalLinkForAuthor(authorDto, linkId, newUrl);
				return Ok($"Inserted link: {res}");
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpDelete(Name = "RemoveExternalLinkForAuthor")]
		public async Task<IActionResult> RemoveExternalLinkForAuthor([FromQuery] AuthorDto authorDto, int linkId, string url) {
			try {
				ExternalLink res = await _business.RemoveExternalLinkForAuthor(authorDto, linkId);
				return Ok($"Inserted link: {res}");
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

		[HttpGet(Name = "GetAuthorFromId")]
		public async Task<IActionResult> GetAuthorFromId(int authorId) {
			try {
				Author author = await _business.GetAuthorFromId(authorId);
				return Ok(author);
			} catch (Exception e) {
				return BadRequest($"{e}");
			}
		}

	}
}