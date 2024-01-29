using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Shared;
using AuthorsHandler.Repository.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthorsHandler.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class AuthorsHandlerController : ControllerBase {
		private readonly IBusiness business_;
		private readonly ILogger logger_;

		public AuthorsHandlerController(IBusiness business, ILogger<AuthorsHandlerController> logger) {
			business_ = business;
			logger_ = logger;
		}

		[HttpPut(Name = "CreateAuthor")]
		public async Task<IActionResult> CreateAuthor([FromQuery] AuthorDto authorDto) {

			bool isCreated = await business_.CreateAuthor(authorDto);

			if (isCreated)
				return Ok("Author " + authorDto.name + " " + authorDto.surname + " added");
			else
				return BadRequest("The author already exists");
		}

		[HttpDelete(Name = "RemoveAuthor")]
		public async Task<IActionResult> DeleteAuthor([FromQuery] AuthorDto authorDto) {

			Author? deletedAuthor = await business_.RemoveAuthor(authorDto);

			if (deletedAuthor != null)
				return Ok("Author " + deletedAuthor.name + " " + deletedAuthor.surname + " deleted");
			else
				return BadRequest("The author does not exist");
		}

		[HttpPost(Name = "UpdateAuthor")]
		public async Task<IActionResult> UpdateAuthor([FromQuery] UpdateAuthorDto update) { // cerca Query
			AuthorDto oldAuthor = new AuthorDto { name = update.oldName, surname = update.oldSurname };
			AuthorDto newAuthor = new AuthorDto { name = update.newName, surname = update.newSurname };

			try {
				var res = await business_.UpdateAuthor(oldAuthor, newAuthor);
				return Ok($"User updated: {update}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpGet(Name = "GetAuthorIdFromName")]
		public async Task<IActionResult> GetAuthorIdFromName(string name, string surname) {
			try {
				int res = await business_.GetAuthorIdFromName(name, surname);
				return Ok($"User id: {res}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpGet(Name = "GetExternalLinksForAuthor")]
		public async Task<IActionResult> GetExternalLinksForAuthor(string name, string surname) {
			ICollection<string>? res = await business_.GetExternalLinksForAuthor(name, surname);

			if (res == null)
				return NotFound("Author " + name + " " + surname + " not found");

			return Ok("Author " + name + " " + surname + " has urls:\n" + String.Join("\n", res));
		}

		[HttpPut(Name = "InsertExternalLink")]
		public async Task<IActionResult> InsertExternalLink([FromQuery] AuthorDto authorDto, string url) {
			try {
				ExternalLink? res = await business_.InsertExternalLinkForAuthor(authorDto, url);
				return Ok($"Inserted link: {res}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpPost(Name = "UpdateExternalLinkForAuthor")]
		public async Task<IActionResult> UpdateExternalLinkForAuthor([FromQuery] AuthorDto authorDto, int linkId, string newUrl) {
			try {
				ExternalLink? res = await business_.UpdateExternalLinkForAuthor(authorDto, linkId, newUrl);
				return Ok($"Inserted link: {res}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpDelete(Name = "RemoveExternalLinkForAuthor")]
		public async Task<IActionResult> RemoveExternalLinkForAuthor([FromQuery] AuthorDto authorDto, int linkId, string url) {
			try {
				ExternalLink? res = await business_.RemoveExternalLinkForAuthor(authorDto, linkId);
				return Ok($"Inserted link: {res}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

		[HttpGet(Name = "GetAuthorFromId")]
		public async Task<IActionResult> GetAuthorFromId(int authorId) {
			try {
				Author author = await business_.GetAuthorFromId(authorId);
				return Ok($"{JsonSerializer.Serialize(author)}");
			} catch (Exception e) {
				return BadRequest($"{e.Message}");
			}
		}

	}
}