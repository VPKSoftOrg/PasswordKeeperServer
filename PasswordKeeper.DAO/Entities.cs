using Microsoft.EntityFrameworkCore;

namespace PasswordKeeper.DAO;

public sealed class Entities : DbContext
{
    public Entities(DbContextOptions<Entities> options) : base(options)
    {
        KeyData = Set<KeyData>();
        Users = Set<User>();
    }

    /// <summary>
    /// The <c>User</c> database table.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// The <c>KeyData</c> database table.
    /// </summary>
    public DbSet<KeyData> KeyData { get; init; }
    
    /// <inheritdoc cref="DbContext.OnModelCreating" />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<KeyData>().HasKey(f => f.Id);
    }
}