namespace AuthorsHandler.Repository.Model
{
	public class Author {
        public int id {get; set;}
        public string name { get; set; }
        public string surname { get; set; }
        public ICollection<ExternalLink> externalLinks = new LinkedList<ExternalLink>();
	}
}