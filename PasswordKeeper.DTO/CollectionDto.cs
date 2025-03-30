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

    /// <summary>
    /// The access key for the collection.
    /// </summary>
    /// <remarks>The access key is only delivered once to the user and it cannot be recovered.</remarks>
    public string? AccessKey { get; set; }
}