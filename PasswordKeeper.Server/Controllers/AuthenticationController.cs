﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordKeeper.BusinessLogic;
using PasswordKeeper.Classes;
using PasswordKeeper.DTO;
using PasswordKeeper.Server.Controllers.Extensions;

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
    /// User change data.
    /// </summary>
    /// <param name="UserId">The ID of the user to change.</param>
    /// <param name="Username">The new username for the user.</param>
    /// <param name="Password">The new password for the user.</param> 
    public record UserChangeRequest(int UserId, string Username, string Password);
    
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
        var userDto = await users.GetUserById(user.UserId);
        
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
        userDto.PasswordHash = Users.HashPassword(user.Password, ref salt);
        userDto.PasswordSalt = Convert.ToBase64String(salt!);
        var result = await users.UpsertUser(userDto);
        
        return result is null ? BadRequest() : Ok(result);
    }
    
    /// <summary>
    /// Updates the user's username if the requester is authorized.
    /// </summary>
    /// <param name="user">The user change request containing the user ID and new username.</param>
    /// <returns>
    /// Unauthorized if the requester is not the user or admin, NotFound if the user does not exist,
    /// otherwise BadRequest if the upsert operation fails, or Ok with the updated user data.
    /// </returns>
    public async Task<IActionResult> UpdateUserName([FromBody] UserChangeRequest user)
    {
        var userDto = await users.GetUserById(user.UserId);

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
        var result = await users.UpsertUser(userDto);
        
        return result is null ? BadRequest() : Ok(result);
    }
    
    /// <summary>
    /// Creates a new user if the requester is admin.
    /// </summary>
    /// <param name="user">The user data to create.</param>
    /// <returns>
    /// Unauthorized if the requester is not admin, BadRequest if the upsert operation fails,
    /// otherwise Ok with the created user data.
    /// </returns>
    public async Task<IActionResult> CreateUser([FromBody] UserChangeRequest user)
    {
        var loggedUser = await users.GetUserById(this.GetLoggedUserId());
        
        if (await users.GetUserByName(user.Username) is not null)
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
            };
        
            var salt = Convert.FromBase64String(userDto.PasswordSalt);
            userDto.PasswordHash = Users.HashPassword(user.Password, ref salt);
            userDto.PasswordSalt = Convert.ToBase64String(salt!);
            var result = await users.UpsertUser(userDto);
        
            return result is null ? BadRequest() : Ok(result);
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
}