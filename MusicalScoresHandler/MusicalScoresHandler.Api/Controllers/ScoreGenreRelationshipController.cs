using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicalScoresHandler.Api.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class ScoreGenreRelationshipController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger<ScoreGenreRelationshipController> _logger;

		public ScoreGenreRelationshipController(IBusiness business, ILogger<ScoreGenreRelationshipController> logger) {
			_business = business;
			_logger = logger;
		}

		[HttpPost(Name="CreateScoreGenreRelationship")]
		public async Task<ActionResult> CreateScoreGenreRelationship([FromQuery] ScoreGenreRelationshipDto relationshipDto, CancellationToken cancellationToken = default) {
			try {
				await _business.CreateScoreGenreRelationship(relationshipDto, cancellationToken);
				_logger.LogInformation($"Created Score-Genre Relationship: ScoreId={relationshipDto.ScoreId}, GenreId={relationshipDto.GenreId}");
				return Ok($"Created Score-Genre Relationship: ScoreId={relationshipDto.ScoreId}, GenreId={relationshipDto.GenreId}");
			} catch (Exception e) {
				_logger.LogError($"Failed to create Score-Genre Relationship: {e}");
				return BadRequest($"Failed to create Score-Genre Relationship: {e}");
			}
		}

		[HttpGet(Name="GetAllScoreGenres")]
		public async Task<ActionResult<List<Genre>>> GetAllScoreGenres([FromQuery]int scoreId, CancellationToken cancellationToken = default) {
			try {
				List<GenreDto> list = await _business.GetAllScoreGenres(scoreId, cancellationToken);
				_logger.LogInformation($"Retrieved {list.Count} genres for ScoreId={scoreId}");
				return Ok($"{JsonSerializer.Serialize(list)}");
			} catch (Exception e) {
				_logger.LogError($"Failed to retrieve genres for ScoreId={scoreId}: {e}");
				return BadRequest($"Failed to retrieve genres for ScoreId={scoreId}: {e}");
			}
		}

		[HttpDelete(Name="DeleteScoreGenreRelationship")]
		public async Task<ActionResult> DeleteScoreGenreRelationship([FromQuery]int id, CancellationToken cancellationToken = default) {
			try {
				var deletedRelationship = await _business.DeleteScoreGenreRelationship(id, cancellationToken);
				_logger.LogInformation($"Deleted Score-Genre Relationship: Id={deletedRelationship.Id}");
				return Ok($"Deleted Score-Genre Relationship: Id={deletedRelationship.Id}");
			} catch (Exception e) {
				_logger.LogError($"Failed to delete Score-Genre Relationship with Id={id}: {e}");
				return BadRequest($"Failed to delete Score-Genre Relationship with Id={id}: {e}");
			}
		}
	}
}
