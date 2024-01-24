using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicalScoreDtosHandler.Shared {
	public class MusicalScoreDto {
		public string Title { get; set; }
		public int AuthorId { get; set; }
		public string? Opus { get; set; }
		public string? Alias { get; set; }
		public string? Description { get; set; }
		public string? YearOfComposition { get; set; }
	}
}