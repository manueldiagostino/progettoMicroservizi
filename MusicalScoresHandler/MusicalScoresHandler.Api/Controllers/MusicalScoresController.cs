using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicalScoreDtosHandler.Shared;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Repository.Model;
using MusicalScoresHandler.Shared;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicalScoresHandler.Api.Controllers {
	[ApiController]
	[Route("[controller]/[action]")]
	public class MusicalScoreController : ControllerBase {
		private readonly IBusiness _business;
		private readonly ILogger<MusicalScoreController> _logger;

		public MusicalScoreController(IBusiness business, ILogger<MusicalScoreController> logger) {
			_business = business;
			_logger = logger;
		}

		[HttpPost(Name="CreateMusicalScore")]
		public async Task<ActionResult> CreateMusicalScore([FromQuery] MusicalScoreDto musicalScoreDto) {
			try {
				await _business.CreateMusicalScore(musicalScoreDto);
				_logger.LogInformation($"Created Musical Score <{JsonSerializer.Serialize(musicalScoreDto)}>");
				return Ok($"Created Musical Score <{JsonSerializer.Serialize(musicalScoreDto)}>");
			} catch (Exception e) {
				_logger.LogError($"Error creating Musical Score: {e}");
				return BadRequest($"Error creating Musical Score: {e.Message}");
			}
		}

		[HttpGet(Name="GetMusicalScoreById")]
		public async Task<ActionResult<MusicalScore>> GetMusicalScoreById([FromQuery]int id) {
			try {
				var musicalScore = await _business.GetMusicalScoreById(id);
				_logger.LogInformation($"Retrieved Musical Score with Id: {id}");
				return Ok($"Retrieved Musical Score with Id: <{id}>\n{JsonSerializer.Serialize(musicalScore)}");
			} catch (Exception e) {
				_logger.LogError($"Error getting Musical Score: {e}");
				return BadRequest($"Error getting Musical Score: {e.Message}");
			}
		}

		[HttpGet(Name="GetAllMusicalScores")]
		public async Task<ActionResult<List<MusicalScore>>> GetAllMusicalScores() {
			try {
				var musicalScores = await _business.GetAllMusicalScores();
				_logger.LogInformation($"Retrieved {musicalScores.Count} Musical Scores");
				return Ok($"{JsonSerializer.Serialize(musicalScores)}");
			} catch (Exception e) {
				_logger.LogError($"Error getting Musical Scores: {e}");
				return BadRequest($"Error getting Musical Scores: {e.Message}");
			}
		}

		[HttpPut(Name="UpdateMusicalScore")]
		public async Task<ActionResult<MusicalScore>> UpdateMusicalScore([FromQuery]int scoreId, [FromQuery]MusicalScoreDto musicalScoreDto) {
			try {
				var updatedMusicalScore = await _business.UpdateMusicalScore(musicalScoreDto);
				_logger.LogInformation($"Updated Musical Score with Id: {updatedMusicalScore.Id}");
				return Ok(updatedMusicalScore);
			} catch (Exception e) {
				_logger.LogError($"Error updating Musical Score: {e}");
				return BadRequest($"Error updating Musical Score: {e.Message}");
			}
		}

		[HttpDelete(Name="DeleteMusicalScore")]
		public async Task<ActionResult<MusicalScore>> DeleteMusicalScore(int id) {
			try {
				var deletedMusicalScore = await _business.DeleteMusicalScore(id);
				_logger.LogInformation($"Deleted Musical Score with Id: {deletedMusicalScore.Id}");
				return Ok(deletedMusicalScore);
			} catch (Exception e) {
				_logger.LogError($"Error deleting Musical Score: {e}");
				return BadRequest($"Error deleting Musical Score: {e.Message}");
			}
		}
	}
}
