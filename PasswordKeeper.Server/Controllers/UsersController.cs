using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordKeeper.BusinessLogic;
using PasswordKeeper.Classes;
using PasswordKeeper.DTO;
using PasswordKeeper.Server.Controllers.Extensions;

namespace PasswordKeeper.Server.Controllers;

/// <summary>
/// The users controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(UsersBusinessLogic usersBusinessLogic) : ControllerBase
{
    /// <summary>
    /// User change data.
    /// </summary>
    /// <param name="UserId">The ID of the user to change.</param>
    /// <param name="Username">The new username for the user.</param>
    /// <param name="Password">The new password for the user.</param>
    /// <param name="UserFullName">The new full name for the user.</param>
    public record UserChangeRequest(int UserId, string Username, string Password, string UserFullName);
    
    /// <summary>
    /// Creates a new user if the requester is admin.
    /// </summary>
    /// <param name="user">The user data to create.</param>
    /// <returns>
    /// Unauthorized if the requester is not admin, BadRequest if the upsert operation fails,
    /// otherwise Ok with the created user data.
    /// </returns>
    [HttpPost]
    [Route(nameof(CreateUser))]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] UserChangeRequest user)
    {
        var loggedUser = await usersBusinessLogic.GetUserById(this.GetLoggedUserId());
        
        if (await usersBusinessLogic.GetUserByName(user.Username) is not null)
        {
            return BadRequest(Passwords.UserAlreadyExists);
        }

        if (loggedUser == null)
        {
            return Unauthorized();
        }

        if (loggedUser.IsAdmin)
        {
            if (!Passwords.IsPasswordOk(user.Password, out var message, out _))
            {
                return BadRequest(message);
            }
            
            if (user.Username.Length < 4)
            {
                return BadRequest(Passwords.UsernameMustBeAtLeast4CharactersLong);
            }
            
            var userDto = new UserDto
            {
                Username = user.Username,
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                IsAdmin = false,
                UserFullName = user.UserFullName,
            };
        
            var salt = string.IsNullOrEmpty(userDto.PasswordSalt) ? null : Convert.FromBase64String(userDto.PasswordSalt);
            userDto.PasswordHash = UsersBusinessLogic.HashPassword(user.Password, ref salt);
            userDto.PasswordSalt = Convert.ToBase64String(salt!);
            var result = await usersBusinessLogic.UpsertUser(userDto);
        
            return result is null ? BadRequest() : Ok(result);
        }

        return Unauthorized();
    }
    
    /// <summary>
    /// Updates the user's password if the requester is authorized and the password meets the complexity requirements.
    /// </summary>
    /// <param name="user">The user change request containing the user ID and new password.</param>
    /// <returns>
    /// Unauthorized if the requester is not the user or admin, BadRequest if the password is invalid or the upsert operation fails,
    /// NotFound if the user does not exist, otherwise Ok with the updated user data.
    /// </returns>
    [Route(nameof(UpdateUserPassword))]
    [HttpPost]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UserChangeRequest user)
    {
        var userDto = await usersBusinessLogic.GetUserById(user.UserId);
        
        if (userDto is null)
        {
            return NotFound();
        }
        
        if (this.GetLoggedUserId() != user.UserId || !userDto.IsAdmin)
        {
            return Unauthorized();
        }
        
        if (!Passwords.IsPasswordOk(user.Password, out var message, out _))
        {
            return BadRequest(message);
        }
        
        var salt = Convert.FromBase64String(userDto.PasswordSalt);
        userDto.PasswordHash = UsersBusinessLogic.HashPassword(user.Password, ref salt);
        userDto.PasswordSalt = Convert.ToBase64String(salt!);
        var result = await usersBusinessLogic.UpsertUser(userDto);
        
        return result is null ? BadRequest() : Ok(result);
    }
    
    /// <summary>
    /// Updates the user's username and full name if the requester is authorized.
    /// </summary>
    /// <param name="user">The user change request containing the user ID, new username and new full name.</param>
    /// <returns>
    /// Unauthorized if the requester is not the user or admin, NotFound if the user does not exist,
    /// otherwise BadRequest if the upsert operation fails, or Ok with the updated user data.
    /// </returns>
    public async Task<IActionResult> UpdateUserName([FromBody] UserChangeRequest user)
    {
        var userDto = await usersBusinessLogic.GetUserById(user.UserId);

        if (userDto is null)
        {
            return NotFound();
        }
        
        if (this.GetLoggedUserId() != user.UserId || !userDto.IsAdmin)
        {
            return Unauthorized();
        }
     
        if (user.Username.Length < 4)
        {
            return BadRequest(Passwords.UsernameMustBeAtLeast4CharactersLong);
        }
        
        userDto.Username = user.Username;
        userDto.UserFullName = user.UserFullName;
        var result = await usersBusinessLogic.UpsertUser(userDto);
        
        return result is null ? BadRequest() : Ok(result);
    }
    
    /// <summary>
    /// Deletes the user with the given ID if the requester is admin.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>Unauthorized if the requester is not admin, NotFound if the user does not exist, otherwise Ok.</returns>
    [Route(nameof(DeleteUser))]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var userDto = await usersBusinessLogic.GetUserById(userId);
        
        if (userDto is null)
        {
            return NotFound();
        }
        
        var loggedUserId = this.GetLoggedUserId();
        var isLoggedUserAdmin = await usersBusinessLogic.GetUserById(loggedUserId) is { IsAdmin: true };
        
        // Only admins can delete users and admins cannot delete themselves
        if (!isLoggedUserAdmin || loggedUserId == userId)
        {
            return Unauthorized();
        }
        
        await usersBusinessLogic.DeleteUser(userId);
        
        return Ok();
    }
    
    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    [Route(nameof(GetAllUsers))]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        
        return await usersBusinessLogic.GetAllUsers();
    }
    
    /// <summary>
    /// Checks a password against the password complexity requirements.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <returns>Ok if the password meets the complexity requirements, otherwise BadRequest with an error message.</returns>
    [Route(nameof(PasswordOk))]
    [HttpPost]
    public IActionResult PasswordOk([FromBody] string password)
    {
        var result = Passwords.IsPasswordOk(password, out var message, out _);
        return result ? Ok() : BadRequest(message);
    }
}