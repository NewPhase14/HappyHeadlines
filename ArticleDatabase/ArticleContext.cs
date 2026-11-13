using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ArticleDatabase;

public class ArticleContext(DbContextOptions<ArticleContext> options) : DbContext(options)
{
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            entity.HasIndex(a => a.CreatedAt);

            entity.ToTable("article");

            entity.Property(a => a.Id)
                .HasColumnName("id");

            entity.Property(a => a.Title)
                .HasColumnName("title");

            entity.Property(a => a.Content)
                .HasColumnName("content");

            entity.Property(a => a.Continent)
                .HasColumnName("continent")
                .HasMaxLength(50);

            entity.Property(a => a.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(a => a.Draft)
                .HasColumnName("draft");
        });
    }
}