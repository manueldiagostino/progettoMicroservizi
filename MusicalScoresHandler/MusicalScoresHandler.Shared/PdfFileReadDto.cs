namespace MusicalScoresHandler.Shared;

public class PdfFileReadDto {
	public int MusicalScoreId { get; set; }
	public string? Publisher { get; set; }
	public int? CopyrightId { get; set; }
	public bool IsUrtext { get; set; }
	public int UserId { get; set; }
	public string? Comments { get; set; }
}
