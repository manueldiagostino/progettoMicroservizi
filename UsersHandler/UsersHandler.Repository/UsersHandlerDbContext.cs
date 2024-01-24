using GlobalUtility.Kafka.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UsersHandler.Repository.Model;

namespace UsersHandler.Repository;

public class UsersHandlerDbContext : DbContext {
	protected readonly IConfiguration Configuration;
	public DbSet<User> Users { get; set; }
	public DbSet<Bio> Biographies { get; set; }
	public DbSet<TransactionalOutbox> TransactionalOutboxes { get; set; }
	public UsersHandlerDbContext(IConfiguration configuration) {
		Configuration = configuration;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {

		modelBuilder.Entity<User>().ToTable("user");
		modelBuilder.Entity<User>().HasKey(x => x.Id);
		modelBuilder.Entity<User>()
			.HasOne(x => x.Bio)
			.WithOne(b => b.User)
			.HasForeignKey<Bio>(b => b.UserId);
		modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
		modelBuilder.Entity<User>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<User>().Property(x => x.Username).HasColumnName("username");
		modelBuilder.Entity<User>().Property(x => x.Name).HasColumnName("name");
		modelBuilder.Entity<User>().Property(x => x.Surname).HasColumnName("surname");
		modelBuilder.Entity<User>().Property(x => x.PropicPath).HasColumnName("propic_path");
		modelBuilder.Entity<User>().Property(x => x.BioId).HasColumnName("bio_id");
		modelBuilder.Entity<User>().Property(x => x.Hash).HasColumnName("hash");
		modelBuilder.Entity<User>().Property(x => x.Salt).HasColumnName("salt");
		modelBuilder.Entity<User>().Property(x => x.Timestamp).HasColumnName("upload_time");


		modelBuilder.Entity<Bio>().ToTable("bio");
		modelBuilder.Entity<Bio>().HasKey(x => x.Id);
		modelBuilder.Entity<Bio>()
			.HasOne(x => x.User)
			.WithOne(u => u.Bio)
			.HasForeignKey<User>(u => u.BioId);
		modelBuilder.Entity<Bio>().Property(x => x.Id).ValueGeneratedOnAdd();
		modelBuilder.Entity<Bio>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<Bio>().Property(x => x.Text).HasColumnName("text");
		modelBuilder.Entity<Bio>().Property(x => x.UserId).HasColumnName("user_id");


		modelBuilder.Entity<TransactionalOutbox>().ToTable("transactional_outbox");
		modelBuilder.Entity<TransactionalOutbox>().HasKey(x => x.id);
		modelBuilder.Entity<TransactionalOutbox>().Property(x => x.id).ValueGeneratedOnAdd();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		// connect to postgres with connection string from app settings
		options.UseNpgsql(Configuration.GetConnectionString("UsersHandlerDbContext"));
	}
}
