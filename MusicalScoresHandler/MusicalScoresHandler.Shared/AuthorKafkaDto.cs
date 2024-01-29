using System.Text.Json.Serialization;

namespace MusicalScoresHandler.Repository.Model {
	public class AuthorKafkaDto {
		[JsonPropertyName("id")]
		public int AuthorId { get; set; }

		public string Name { get; set; }
		public string Surname { get; set; }
	}
}