using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                var token = Helpers.GenerateJwtToken(user.Username, userDto.Id);
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
}