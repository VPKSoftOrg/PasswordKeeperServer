using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>User</c> database table.
/// </summary>
public class User : IUser
{
    /// <inheritdoc cref="IUser.Id" />
    public long Id { get; set; }
    
    /// <inheritdoc cref="IUser.UserName" />
    public string UserName { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.Password" />
    public string Password { get; set; } = string.Empty;
}