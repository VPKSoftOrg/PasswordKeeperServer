namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the shared members of the <c>Collection</c> database table DAO/TDO shared members.
/// </summary>
public interface ICollection : IHasId
{
    /// <summary>
    /// The collection name.
    /// </summary>
    string Name { get; set; }
}