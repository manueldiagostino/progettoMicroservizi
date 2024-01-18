using AuthorsHandler.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthorsHandler.Repository
{	
	public class AuthorsHandlerDbContext : DbContext {
		protected readonly IConfiguration Configuration;
		public AuthorsHandlerDbContext(IConfiguration configuration) {
			Configuration = configuration; // injection
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

			modelBuilder.Entity<Author>().ToTable("author");
			modelBuilder.Entity<ExternalLink>().ToTable("external_link");

			modelBuilder.Entity<Author>().HasKey(x => x.id);
			modelBuilder.Entity<ExternalLink>().HasKey(x => x.id);

			modelBuilder.Entity<Author>()
				.HasMany(e => e.externalLinks)
				.WithOne(e => e.author);
        }

		public DbSet<Author> Authors {get; set;}
		public DbSet<ExternalLink> ExternalLinks {get; set;}


		protected override void OnConfiguring(DbContextOptionsBuilder options) {
			// connect to postgres with connection string from app settings
			options.UseNpgsql(Configuration.GetConnectionString("AuthorsHandlerDbContext"));
    	}
	}
}