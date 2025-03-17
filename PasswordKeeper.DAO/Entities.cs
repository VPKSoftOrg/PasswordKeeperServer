using Microsoft.EntityFrameworkCore;

namespace PasswordKeeper.DAO;

public sealed class Entities : DbContext
{
    public Entities(DbContextOptions<Entities> options) : base(options)
    {
        KeyData = Set<KeyData>();
    }
    
    public DbSet<KeyData> KeyData { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<KeyData>().HasKey(f => f.Id);
    }
}