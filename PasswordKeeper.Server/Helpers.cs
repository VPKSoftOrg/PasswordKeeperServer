using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PasswordKeeper.Server;

/// <summary>
/// Some static helper methods.
/// </summary>
public class Helpers
{
    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The username to generate the JWT token for.</param>
    /// <param name="userId">The user ID to generate the JWT token for.</param>
    /// <returns>The JWT token.</returns>
    public static string GenerateJwtToken(string username, long userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
        };
        
        var key = new SymmetricSecurityKey(Program.JwtKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Program.PseudoDomain,
            audience: Program.PseudoDomain,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    /// <summary>
    /// Checks if the given password is valid according to the following rules:
    /// <list type="bullet">
    /// <item>Must be at least 8 characters long.</item>
    /// <item>Must contain at least one uppercase letter.</item>
    /// <item>Must contain at least one lowercase letter.</item>
    /// <item>Must contain at least one number.</item>
    /// <item>Must contain at least one special character.</item>
    /// </list>
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <param name="message">The error message if the password is not valid.</param>
    /// <returns><c>true</c> if the password is valid according to the rules; otherwise, <c>false</c>.</returns>
    public static bool IsPasswordOk(string password, out string message)
    {
        const string specialCharacters = @"!@#$%^&*()_+/\-=[]{}|;:,.<>?~";
        
        if (password.Length < 8)
        {
            message = "Password must be at least 8 characters long.";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            message = "Password must contain at least one uppercase letter.";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            message = "Password must contain at least one lowercase letter.";
            return false;
        }
        
        if (!password.Any(char.IsDigit))
        {
            message = "Password must contain at least one number.";
            return false;
        }
        
        if (!password.Any(c => specialCharacters.Contains(c)))
        {
            message = "Password must contain at least one special character.";
            return false;
        }

        message = string.Empty;
        
        return true;
    }
}