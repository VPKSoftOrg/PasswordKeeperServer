namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for a DTO/DAO having a CollectionId field.
/// </summary>
public interface IHasCollectionId
{
    /// <summary>
    /// The ID of the collection.
    /// </summary>
    long CollectionId { get; set; }
}