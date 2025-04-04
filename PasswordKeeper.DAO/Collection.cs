using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>Collection</c> database table.
/// </summary>
[Table(nameof(Collection))]
public class Collection : ICollection, IKeyHashMembers
{
    /// <inheritdoc cref="IHasId.Id" />
    public long Id { get; set; }

    /// <inheritdoc cref="ICollection.Name" />
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    /// <inheritdoc cref="IKeyHashMembers.ChallengeKeyHash" />
    [MaxLength(1000)]
    public string ChallengeKeyHash { get; set; } = string.Empty;

    /// <inheritdoc cref="IKeyHashMembers.ChallengeKeySalt" />
    [MaxLength(1000)]
    public string ChallengeKeySalt { get; set; } = string.Empty;

    /// <summary>
    /// A related <c>CollectionSettings</c> object.
    /// </summary>
    public CollectionSettings? CollectionSettings { get; set; }
    
    /// <summary>
    /// A related collection of <c>CollectionItem</c> objects.
    /// </summary>
    public ICollection<CollectionItem>? CollectionItems { get; set; } 
}