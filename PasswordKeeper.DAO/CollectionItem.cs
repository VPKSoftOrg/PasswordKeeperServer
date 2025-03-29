using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>CollectionItem</c> database table.
/// </summary>
public class CollectionItem : IHasId
{
    /// <inheritdoc cref="IHasId.Id"/>
    public long Id { get; set; }
    
    /// <summary>
    /// The ID of the collection this item belongs to.
    /// </summary>
    public long CollectionId { get; set; }
    
    /// <summary>
    /// The data of the item.
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string ItemData { get; set; } = string.Empty;
    
    /// <summary>
    /// The collection this item belongs to.
    /// </summary>
    public Collection? Collection { get; set; }
}