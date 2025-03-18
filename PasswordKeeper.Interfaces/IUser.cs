namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the shared members of the <c>User</c> database table DAO/TDO shared members.
/// </summary>
public interface IUser
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    long Id { get; set; }
    
    /// <summary>
    /// The name of the user.
    /// </summary>
    string UserName { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    string PasswordHash { get; set; }
    
    /// <summary>
    /// The salt of the user's password.
    /// </summary>
    string PasswordSalt { get; set; }
}