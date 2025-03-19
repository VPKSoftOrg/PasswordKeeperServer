using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PasswordKeeperServer;

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
}