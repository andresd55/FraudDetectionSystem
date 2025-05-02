using Microsoft.EntityFrameworkCore;

using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Value)
            .HasColumnType("numeric(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}
