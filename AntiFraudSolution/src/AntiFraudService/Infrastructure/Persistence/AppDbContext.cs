using Microsoft.EntityFrameworkCore;
using AntiFraudService.Domain.Entities;

namespace AntiFraudService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<TransactionValidation> Validations => Set<TransactionValidation>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionValidation>()
            .Property(t => t.Value)
            .HasColumnType("numeric(18,2)");
    }
}
