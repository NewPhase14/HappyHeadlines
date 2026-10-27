using CoreDomain;
using Microsoft.EntityFrameworkCore;

namespace SubscriberDatabase;

public class SubscriberContext(DbContextOptions<SubscriberContext> options) : DbContext(options)
{
    public DbSet<Subscriber> Subscribers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(sub => sub.Email);

            entity.ToTable("subscriber");

            entity.Property(sub => sub.Email)
                .HasColumnName("email");
        });
    }
}