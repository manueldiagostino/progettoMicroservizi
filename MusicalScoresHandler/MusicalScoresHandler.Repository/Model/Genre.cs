namespace MusicalScoresHandler.Repository.Model;

public class Genre {
	public int Id { get; set; }
	public string Name { get; set; }

	public ICollection<ScoreGenreRelationship> ScoreGenreRelationship { get; set; }
}
