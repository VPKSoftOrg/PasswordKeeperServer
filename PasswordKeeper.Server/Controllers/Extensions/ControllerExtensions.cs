﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PasswordKeeper.Server.Controllers.Extensions;

/// <summary>
/// Extension methods for the <see cref="ControllerBase"/> class.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Gets the logged-in user's ID.
    /// </summary>
    /// <returns>The logged-in user's ID, or -1 if the claim containing the user ID is not found.</returns>
    public static long GetLoggedUserId(this ControllerBase controllerBase)
    {
        return GetLoggedUserIdFunc(controllerBase);
    }

    /// <summary>
    /// A delegate to get the logged-in user's ID. This is called by the <see cref="GetLoggedUserId"/> method.
    /// </summary>
    /// <remarks>A delegate is used to allow unit testing.</remarks>
    public static Func<ControllerBase, long> GetLoggedUserIdFunc = controllerBase =>
    {
        var claim = controllerBase.User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier && long.TryParse(c.Value, out _));

        if (long.TryParse(claim?.Value, out var result))
        {
            return result;
        }

        return -1;
    }; 
    
    /// <summary>
    /// Gets the logged-in user's name.
    /// </summary>
    /// <returns>The logged-in user's name, or an empty string if the user is not logged in.</returns>
    public static string GetLoggedUserName(this ControllerBase controllerBase)
    {
        return GetLoggedUserNameFunc(controllerBase);
    }
    
    /// <summary>
    /// A delegate to get the logged-in user's name. This is called by the <see cref="GetLoggedUserName"/> method.
    /// </summary>
    /// <remarks>A delegate is used to allow unit testing.</remarks>
    public static Func<ControllerBase, string> GetLoggedUserNameFunc = controllerBase =>
    {
        return controllerBase.User.Identity?.Name ?? string.Empty;
    };
}