namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the <c>CollectionSettings</c> database table.
/// </summary>
public interface ICollectionSettings : IHasId, IHasCollectionId
{
    /// <summary>
    /// The JSON settings for the collection.
    /// </summary>
    string JsonSettings { get; set; }
}