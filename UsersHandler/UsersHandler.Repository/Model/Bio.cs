namespace UsersHandler.Repository.Model;

public class Bio {
	public int Id { get; set; }
	public string? Text { get; set; }
	public int UserId { get; set; }
	public User User { get; set; }
}

