using Microsoft.AspNetCore.Mvc;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MusicalScoresHandler.Repository.Model;
using System.Text.Json;

namespace MusicalScoresHandler.Api.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class GenresController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger _logger;

		public GenresController(IBusiness business, ILogger<GenresController> logger) {
			_business = business;
			_logger = logger;
		}

		[HttpPost(Name = "CreateGenre")]
		public async Task<ActionResult> CreateGenre([FromQuery] GenreDto genreDto) {
			try {
				await _business.CreateGenre(genreDto);
				return Ok($"Created Genre <{genreDto.Name}>");
			} catch (Exception e) {
				_logger.LogError($"Error creating Genre: {e}");
				return BadRequest($"Error creating Genre: {e.Message}");
			}
		}

		[HttpGet(Name = "GetAllGenres")]
		public async Task<ActionResult<List<Genre>>> GetAllGenres() {
			try {
				var genres = await _business.GetAllGenres();
				return Ok(JsonSerializer.Serialize(genres));
			} catch (Exception e) {
				_logger.LogError($"Error getting all genres: {e}");
				return BadRequest($"Error getting all genres: {e.Message}");
			}
		}

		[HttpGet(Name = "GetGenreByName")]
		public async Task<ActionResult<Genre>> GetGenreByName([FromQuery] string name) {
			try {
				var genre = await _business.GetGenreByName(name);

				return Ok(genre);
			} catch (Exception e) {
				_logger.LogError($"Error getting genre by name: {e}");
				return BadRequest($"Error getting genre by name: {e.Message}");
			}
		}

		[HttpPut(Name = "UpdateGenre")]
		public async Task<ActionResult> UpdateGenre([FromQuery] GenreDto genreDto) {
			try {
				var updatedGenre = await _business.UpdateGenre(genreDto);
				return Ok($"Updated Genre <{genreDto.Name}> to {updatedGenre.Name}");
			} catch (Exception e) {
				_logger.LogError($"Error updating Genre: {e}");
				return BadRequest($"Error updating Genre: {e.Message}");
			}
		}

		[HttpDelete(Name = "DeleteGenre")]
		public async Task<ActionResult> DeleteGenre([FromQuery] string name) {
			try {
				var deletedGenre = await _business.DeleteGenre(name);
				return Ok($"Deleted Genre <{deletedGenre.Name}>");
			} catch (Exception e) {
				_logger.LogError($"Error deleting Genre: {e}");
				return BadRequest($"Error deleting Genre: {e.Message}");
			}
		}
	}
}
