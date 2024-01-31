namespace MusicalScoresHandler.Repository.Model;

public class UserKafka {
	public int Id { get; set; }
	public int UserId { get; set; }
	public string Username { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
}
