using System.ComponentModel.DataAnnotations;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>Collection</c> database table.
/// </summary>
public class Collection : ICollection
{
    /// <inheritdoc cref="IHasId.Id" />
    public long Id { get; set; }

    /// <inheritdoc cref="ICollection.Name" />
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}