namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for a DTO/DAO having an ID field.
/// </summary>
public interface IHasId
{
    /// <summary>
    /// The ID of the object.
    /// </summary>
    long Id { get; set; }
}