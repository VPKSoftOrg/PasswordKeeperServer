namespace PasswordKeeper.DAO;

/// <summary>
/// 
/// </summary>
public class KeyData
{
    public long Id { get; set; }

    public string JwtSecurityKey { get; set; } = string.Empty;
}