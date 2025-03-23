using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordKeeper.BusinessLogic;

namespace PasswordKeeper.Server.Controllers;

/// <summary>
/// The authentication controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    [Route(nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLogin user)
    {
        var result = await users.Login(user.Username, user.Password, Program.GetJwtKey(), Program.PseudoDomain);
        
        if (result.Success)
        {
            return Ok(new { token = result.Message, });
        } 
        if (result is { Success: false, Unauthorized: false, })
        {
            return BadRequest(result.Message);
        }

        return Unauthorized();
    }
    
    /// <summary>
    /// An endpoint for testing unauthorized access.
    /// </summary>
    /// <returns>An Ok result if the request is authorized.</returns>
    [Route(nameof(TestUnauthorized))]
    public IActionResult TestUnauthorized()
    {
        return Ok();
    }
    
    /// <summary>
    /// An endpoint for testing admin access.
    /// </summary>
    /// <returns>An Ok result if the request is authorized.</returns>
    [Route(nameof(TestAdminAuthorized))]
    [Authorize(Roles = "Admin")]
    public IActionResult TestAdminAuthorized()
    {
        return Ok();
    }
}