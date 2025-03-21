using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DTO;

/// <summary>
/// A DTO for the <c>Collection</c> database table data.
/// </summary>
public class CollectionDto : ICollection
{
    /// <inheritdoc cref="IHasId.Id"/>
    public long Id { get; set; }

    /// <inheritdoc cref="ICollection.Name"/>
    public string Name { get; set; } = string.Empty;
}