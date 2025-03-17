namespace PasswordKeeper.Interfaces;

/// <summary>
/// An interface for the shared members of the <c>User</c> database table DAO/TDO shared members.
/// </summary>
public interface IUser
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// The name of the user.
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; set; }
}