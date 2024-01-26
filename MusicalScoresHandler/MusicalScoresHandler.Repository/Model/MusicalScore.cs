namespace MusicalScoresHandler.Repository.Model;

public class MusicalScore {
	public int Id { get; set; }
	public string Title { get; set; }
	public string? Alias { get; set; }
	public string? YearOfComposition { get; set; }
	public string? Description { get; set; }
	public string? Opus { get; set; }
	public int AuthorId { get; set; }
	
	public ICollection<PdfFile> PdfFiles { get; set; }
	public ICollection<ScoreGenreRelationship> ScoreGenreRelationship { get; set; }
}
