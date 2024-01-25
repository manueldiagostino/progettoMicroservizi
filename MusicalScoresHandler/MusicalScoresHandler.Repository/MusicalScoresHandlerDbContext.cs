using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository;

public class MusicalScoresHandlerDbContext : DbContext {
	protected readonly IConfiguration Configuration;
	public DbSet<Genre> Genres { get; set; }
	public DbSet<MusicalScore> MusicalScores { get; set; }
	public DbSet<PdfFile> PdfFiles { get; set; }
	public DbSet<ScoreGenreRelationship> ScoreGenreRelationships { get; set; }
	public DbSet<Copyright> Copyrights { get; set; }
	
	public MusicalScoresHandlerDbContext(IConfiguration configuration) {
		Configuration = configuration;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ScoreGenreRelationship>().ToTable("score_genre_relathionship");
		modelBuilder.Entity<ScoreGenreRelationship>().HasKey(x => x.Id);
		modelBuilder.Entity<ScoreGenreRelationship>()
			.HasOne(x => x.Genre)
			.WithMany(g => g.ScoreGenreRelationship)
			.HasForeignKey(x => x.GenreId)
			.OnDelete(DeleteBehavior.Cascade);
		modelBuilder.Entity<ScoreGenreRelationship>()
			.HasOne(x => x.MusicalScore)
			.WithMany(s => s.ScoreGenreRelationship)
			.HasForeignKey(x => x.ScoreId)
			.OnDelete(DeleteBehavior.Cascade);
		modelBuilder.Entity<ScoreGenreRelationship>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<ScoreGenreRelationship>().Property(x => x.ScoreId).HasColumnName("score_id");
		modelBuilder.Entity<ScoreGenreRelationship>().Property(x => x.GenreId).HasColumnName("genre_id");


		modelBuilder.Entity<Genre>().ToTable("genre");
		modelBuilder.Entity<Genre>().HasKey(x => x.Id);
		modelBuilder.Entity<Genre>()
			.HasMany(x => x.ScoreGenreRelationship)
			.WithOne(sgr => sgr.Genre);
		modelBuilder.Entity<Genre>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<Genre>().Property(x => x.Name).HasColumnName("name");


		modelBuilder.Entity<MusicalScore>().ToTable("musical_score");
		modelBuilder.Entity<MusicalScore>().HasKey(x => x.Id);
		modelBuilder.Entity<MusicalScore>()
			.HasMany(x => x.ScoreGenreRelationship)
			.WithOne(sgr => sgr.MusicalScore);
		modelBuilder.Entity<MusicalScore>()
			.HasMany(x => x.PdfFiles)
			.WithOne(f => f.MusicalScore);

		modelBuilder.Entity<MusicalScore>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<MusicalScore>().Property(x => x.Title).HasColumnName("title");
		modelBuilder.Entity<MusicalScore>().Property(x => x.Alias).HasColumnName("alias");
		modelBuilder.Entity<MusicalScore>().Property(x => x.YearOfComposition).HasColumnName("year_of_composition");
		modelBuilder.Entity<MusicalScore>().Property(x => x.Description).HasColumnName("description");
		modelBuilder.Entity<MusicalScore>().Property(x => x.Opus).HasColumnName("opus");
		modelBuilder.Entity<MusicalScore>().Property(x => x.AuthorId).HasColumnName("author_id");
		
		
		modelBuilder.Entity<PdfFile>().ToTable("pdf_file");
		modelBuilder.Entity<PdfFile>().HasKey(x => x.Id);
		modelBuilder.Entity<PdfFile>()
			.HasOne(x => x.MusicalScore)
			.WithMany(s => s.PdfFiles)
			.HasForeignKey(x => x.MusicalScoreId)
			.OnDelete(DeleteBehavior.Cascade);
		modelBuilder.Entity<PdfFile>()
			.HasOne(x => x.Copyright)
			.WithMany(f => f.PdfFiles)
			.HasForeignKey(x => x.CopyrightId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<PdfFile>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<PdfFile>().Property(x => x.MusicalScoreId).HasColumnName("musical_score_id");
		modelBuilder.Entity<PdfFile>().Property(x => x.Path).HasColumnName("path");
		modelBuilder.Entity<PdfFile>().Property(x => x.UploadDate).HasColumnName("upload_date");
		modelBuilder.Entity<PdfFile>().Property(x => x.Publisher).HasColumnName("publisher");
		modelBuilder.Entity<PdfFile>().Property(x => x.CopyrightId).HasColumnName("copyright_id");
		modelBuilder.Entity<PdfFile>().Property(x => x.IsUrtext).HasColumnName("is_urtext");
		modelBuilder.Entity<PdfFile>().Property(x => x.UserId).HasColumnName("user_id");
		modelBuilder.Entity<PdfFile>().Property(x => x.Comments).HasColumnName("comments");
	

		modelBuilder.Entity<Copyright>().ToTable("copyright");
		modelBuilder.Entity<Copyright>().HasKey(x => x.Id);
		modelBuilder.Entity<Copyright>()
			.HasMany(x => x.PdfFiles)
			.WithOne(f => f.Copyright);

		modelBuilder.Entity<Copyright>().Property(x => x.Id).HasColumnName("id");
		modelBuilder.Entity<Copyright>().Property(x => x.Name).HasColumnName("name");
		
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		// connect to postgres with connection string from app settings
		options.UseNpgsql(Configuration.GetConnectionString("UsersHandlerDbContext"));
	}
}
