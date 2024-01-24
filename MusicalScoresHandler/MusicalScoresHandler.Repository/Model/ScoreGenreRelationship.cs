namespace MusicalScoresHandler.Repository.Model;

public class ScoreGenreRelationship {
	public int Id { get; set; }
	public int ScoreId { get; set; }
	public int GenreId { get; set; }

	public MusicalScore MusicalScore { get; set; }
	public Genre Genre { get; set; }
}
