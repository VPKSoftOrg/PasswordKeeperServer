namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for a DTO/DAO having a UserId field.
/// </summary>
public interface IHasUserId
{
    /// <summary>
    /// The ID of the user.
    /// </summary>
    long UserId { get; set; }
}