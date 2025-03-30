using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>UserCollectionMember</c> database table.
/// </summary>
public class UserCollectionMember : IHasId, IHasUserId, IHasCollectionId
{
    /// <inheritdoc cref="IHasId.Id"/>
    public long Id { get; set; }
    
    /// <inheritdoc cref="IHasUserId.UserId"/>
    public long UserId { get; set; }
    
    /// <inheritdoc cref="IHasCollectionId.CollectionId"/>
    public long CollectionId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the collection is the default for the user.
    /// </summary> 
    public bool IsDefaultForUser { get; set; }

    /// <summary>
    /// Gets or sets the collection.
    /// </summary>
    [ForeignKey(nameof(CollectionId))]
    public Collection? Collection { get; set; }
}