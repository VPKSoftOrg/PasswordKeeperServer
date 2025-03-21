using Microsoft.EntityFrameworkCore;

namespace PasswordKeeper.DAO;

/// <summary>
/// The <c>Entities</c> class represents the root entity object for the database context.
/// </summary>
public sealed class Entities : DbContext
{
    /// <summary>
    /// The <c>Entities</c> class represents the root entity object for the database context.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public Entities(DbContextOptions<Entities> options) : base(options)
    {
        KeyData = Set<KeyData>();
        Users = Set<User>();
        Collections = Set<Collection>();
        CollectionSettings = Set<CollectionSettings>();
    }

    /// <summary>
    /// The <c>CollectionSettings</c> database table.
    /// </summary>
    public DbSet<CollectionSettings> CollectionSettings { get; set; }

    /// <summary>
    /// The <c>Collection</c> database table.
    /// </summary>
    public DbSet<Collection> Collections { get; set; }

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
        
        modelBuilder.Entity<User>(f =>
        {
            f.HasKey(i => i.Id);
        });
        
        modelBuilder.Entity<Collection>().HasKey(f => f.Id);
    }
}