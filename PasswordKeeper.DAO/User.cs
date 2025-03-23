using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PasswordKeeper.Interfaces;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>User</c> database table.
/// </summary>
[Table(nameof(User))]
public class User : IUser
{
    /// <inheritdoc cref="IHasId.Id" />
    public long Id { get; set; }
    
    /// <inheritdoc cref="IUser.Username" />
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.UserFullName" />
    [MaxLength(512)]
    public string UserFullName { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.PasswordHash" />
    [MaxLength(1000)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <inheritdoc cref="IUser.PasswordSalt" />
    [MaxLength(1000)]
    public string PasswordSalt { get; set; } = string.Empty;
    
    /// <inheritdoc cref="IUser.IsAdmin" />
    public bool IsAdmin { get; set; }
}