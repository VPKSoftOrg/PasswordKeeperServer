namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the shared members of the <c>User</c> database table DAO/TDO shared members.
/// </summary>
public interface IUser : IHasId
{
    /// <summary>
    /// The name of the user.
    /// </summary>
    string Username { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    string PasswordHash { get; set; }
    
    /// <summary>
    /// The salt of the user's password.
    /// </summary>
    string PasswordSalt { get; set; }
    
    /// <summary>
    /// Whether the user is an admin.
    /// </summary>
    bool IsAdmin { get; set; }
}