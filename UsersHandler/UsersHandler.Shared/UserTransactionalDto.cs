namespace UsersHandler.Shared;

public class UserTransactionalDto {
	public int UserId { get; set; }
	public string Username { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
}
