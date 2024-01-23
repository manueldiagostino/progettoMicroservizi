using GlobalUtility.Kafka.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UsersHandler.Repository.Model;

namespace UsersHandler.Repository;

public class UsersHandlerDbContext : DbContext {
	protected readonly IConfiguration Configuration;
	public DbSet<User> Authors { get; set; }
	public DbSet<Bio> Biographies { get; set; }
	public DbSet<TransactionalOutbox> TransactionalOutboxes { get; set; }
	public UsersHandlerDbContext(IConfiguration configuration) {
		Configuration = configuration;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {

		modelBuilder.Entity<User>().ToTable("author");
		modelBuilder.Entity<User>().HasKey(x => x.id);
		modelBuilder.Entity<User>()
			.HasOne(x => x.bio)
			.WithOne(b => b.user)
			.HasForeignKey<Bio>(b => b.user_id);


		modelBuilder.Entity<Bio>().ToTable("external_link");
		modelBuilder.Entity<Bio>().HasKey(x => x.id);
		modelBuilder.Entity<Bio>()
			.HasOne(x => x.user)
			.WithOne(u => u.bio)
			.HasForeignKey<User>(u => u.bio_id);


		modelBuilder.Entity<TransactionalOutbox>().ToTable("transactional_outbox");
		modelBuilder.Entity<TransactionalOutbox>().HasKey(x => x.id);
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		// connect to postgres with connection string from app settings
		options.UseNpgsql(Configuration.GetConnectionString("UsersHandlerDbContext"));
	}
}
