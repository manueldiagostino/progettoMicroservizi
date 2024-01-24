namespace MusicalScoresHandler.Repository.Model;

public class Copyright {
	public int Id { get; set; }
	public string Name { get; set; }

	public ICollection<PdfFile> PdfFiles;
}
