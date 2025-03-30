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
    
    /// <summary>
    /// The <c>CollectionItem</c> database table.
    /// </summary>
    public DbSet<CollectionItem> CollectionItems { get; init; } = null!;
    
    /// <summary>
    /// The <c>UserCollectionMember</c> database table.
    /// </summary>
    public DbSet<UserCollectionMember> UserCollectionMembers { get; init; } = null!;
}