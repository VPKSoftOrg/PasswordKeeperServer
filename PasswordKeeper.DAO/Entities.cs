using Microsoft.EntityFrameworkCore;

namespace PasswordKeeper.DAO;

/// <summary>
/// The <c>Entities</c> class represents the root entity object for the database context.
/// </summary>
public class Entities : DbContext
{
    /// <summary>
    /// The <c>Entities</c> class represents the root entity object for the database context.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public Entities(DbContextOptions<Entities> options) : base(options)
    {

    }

    /// <summary>
    /// The <c>CollectionSettings</c> database table.
    /// </summary>
    public DbSet<CollectionSettings> CollectionSettings { get; set; } = null!;

    /// <summary>
    /// The <c>Collection</c> database table.
    /// </summary>
    public DbSet<Collection> Collections { get; set; } = null!;

    /// <summary>
    /// The <c>User</c> database table.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// The <c>KeyData</c> database table.
    /// </summary>
    public DbSet<KeyData> KeyData { get; init; } = null!;
    
    /// <inheritdoc cref="DbContext.OnModelCreating" />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<KeyData>().HasKey(f => f.Id);
        
        modelBuilder.Entity<User>(f =>
        {
            f.HasKey(k => k.Id);
        });

        modelBuilder.Entity<Collection>(f =>
        {
            f.HasKey(k => k.Id);
            // Configure one-to-many relationship to CollectionItem
            f.HasMany(k => k.CollectionItems)
                .WithOne(ci => ci.Collection)
                .HasForeignKey(ci => ci.CollectionId);
            
            // Configure one-to-one relationship to CollectionSettings
            f.HasOne(k => k.CollectionSettings)
                .WithOne(cs => cs.Collection)
                .HasForeignKey<CollectionSettings>(cs => cs.CollectionId);
        });
        
        modelBuilder.Entity<CollectionSettings>(f =>
        {
            f.HasKey(k => k.Id);
            // Configure one-to-one relationship to Collection
            f.HasOne(k => k.Collection)
                .WithOne(c => c.CollectionSettings)
                .HasForeignKey<CollectionSettings>(k => k.CollectionId);
        });

        modelBuilder.Entity<CollectionItem>(f =>
        {
            f.HasKey(k => k.Id);
        });
    }
}