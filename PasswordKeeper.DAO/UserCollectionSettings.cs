using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>UserCollectionSettings</c> database table.
/// </summary>
public class UserCollectionSettings : IUserCollectionSettings
{
    /// <inheritdoc cref="IHasId.Id"/>
    public long Id { get; set; }

    /// <inheritdoc cref="ICollectionSettings.JsonSettings"/>
    [MaxLength(int.MaxValue)]
    public string JsonSettings { get; set; } = string.Empty;
    
    /// <inheritdoc cref="IHasUserId.UserId"/>
    public long UserId { get; set; }
    
    /// <inheritdoc cref="IHasCollectionId.CollectionId"/>
    public long CollectionId { get; set; }
    
    /// <summary>
    /// The user associated with the collection settings.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    /// <summary>
    /// The <c>Collection</c> this <c>CollectionSettings</c> is associated with.
    /// </summary>
    [ForeignKey(nameof(CollectionId))]
    public Collection? Collection { get; set; }
}