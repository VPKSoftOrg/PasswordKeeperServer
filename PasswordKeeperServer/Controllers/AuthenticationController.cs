using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PasswordKeeper.BusinessLogic;
using PasswordKeeper.DTO;

namespace PasswordKeeperServer.Controllers;

/// <summary>
/// The authentication controller.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthenticationController(Users users) : ControllerBase
{
    /// <summary>
    /// User login data.
    /// </summary>
    /// <param name="Username">The username for the login.</param>
    /// <param name="Password">The password for the login.</param>
    public record UserLogin(string Username, string Password);
    
    
    /// <summary>
    /// Logs the user in and returns a JWT token.
    /// </summary>
    /// <param name="user">The user login data.</param>
    /// <returns>A JWT token if the login is valid, otherwise Unauthorized.</returns>
    [HttpPost(Name = "Login")]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLogin user)
    {
        var userDto = await users.GetUserByName(user.Username);
        
        // If the username is "admin" and the password is not empty, create a new user,
        // this is the initial admin user.
        if (user is { Username: "admin", Password.Length: > 0, } && userDto is null)
        {
            byte []? salt = null;
            userDto = new UserDto
            {
                UserName = user.Username,
                PasswordHash = Users.HashPassword(user.Password, ref salt),
                PasswordSalt = Convert.ToBase64String(salt!),
            };

            userDto = await users.UpsertUser(userDto);
            
            var token = GenerateJwtToken(user.Username, userDto.Id);
            return Ok(new { token, });
        }

        // An existing user, verify the password
        if (userDto is not null)
        {
            if (Users.VerifyPassword(user.Password, userDto.PasswordHash,
                    Convert.FromBase64String(userDto.PasswordSalt)))
            {
                var token = GenerateJwtToken(user.Username, userDto.Id);
                return Ok(new { token, });
            }
        }

        return Unauthorized();
    }
    
    /// <summary>
    /// An endpoint for testing unauthorized access.
    /// </summary>
    /// <returns>An Ok result if the request is authorized.</returns>
    [Route("test-unauthorized")]
    public IActionResult TestUnauthorized()
    {
        return Ok();
    }
    
    /// <summary>
    /// Gets the logged-in user's ID.
    /// </summary>
    /// <returns>The logged-in user's ID, or -1 if the claim containing the user ID is not found.</returns>
    long GetLoggedUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && long.TryParse(c.Value, out _));
        
        if (long.TryParse(claim?.Value, out var result))
        {
            return result;
        }
        
        return -1;
    }
    
    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The username to generate the JWT token for.</param>
    /// <param name="userId">The user ID to generate the JWT token for.</param>
    /// <returns>The JWT token.</returns>
    private string GenerateJwtToken(string username, long userId)
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