namespace AuthorsHandler.Repository.Model
{
	public class ExternalLink {
		public int id {get; set;}
		public int authorId {get; set;}
		public string url {get; set;}

		public Author author;
	}
}