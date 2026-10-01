using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace ProfanityDatabase;

public class ProfanityContext(DbContextOptions<ProfanityContext> options) : DbContext(options)
{
    public DbSet<Profanity> Profanity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profanity>(entity =>
        {
            entity.ToTable("profanity");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .HasColumnName("id");

            entity.Property(p => p.Word)
                .HasColumnName("word");
        });
    }
}