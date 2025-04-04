using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>CollectionSettings</c> database table.
/// </summary>
[Table(nameof(CollectionSettings))]
public class CollectionSettings : ICollectionSettings
{
    /// <inheritdoc cref="IHasId.Id"/>
    public long Id { get; set; }

    /// <inheritdoc cref="ICollectionSettings.JsonSettings"/>
    [MaxLength(int.MaxValue)]
    public string JsonSettings { get; set; } = string.Empty;

    /// <inheritdoc cref="IHasCollectionId.CollectionId"/>
    public long CollectionId { get; set; }
    
    /// <summary>
    /// The <c>Collection</c> this <c>CollectionSettings</c> is associated with.
    /// </summary>
    [ForeignKey(nameof(CollectionId))]
    public Collection? Collection { get; set; }
}