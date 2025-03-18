using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordKeeper.DAO;

/// <summary>
/// A DAO for the <c>KeyData</c> database table.
/// </summary>
[Table(nameof(KeyData))]
public class KeyData
{
    /// <summary>
    /// The primary key for the <c>KeyData</c> database table.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The JWT security key.
    /// </summary>
    [MaxLength(684)]
    public string JwtSecurityKey { get; set; } = string.Empty;
}