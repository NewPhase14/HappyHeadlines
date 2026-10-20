using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace CommentDatabase;

public class CommentContext(DbContextOptions<CommentContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comment");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("id");

            entity.Property(c => c.ArticleId)
                .HasColumnName("article_id");

            entity.Property(c => c.Message)
                .HasColumnName("message");

            entity.Property(c => c.Author)
                .HasColumnName("author");

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone");
        });
    }
}