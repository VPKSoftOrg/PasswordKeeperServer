using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PasswordKeeper.Server.Controllers;

/// <summary>
/// The alive controller.
/// </summary>
public class AliveController : ControllerBase
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    /// <returns>The current date and time.</returns>
    [Route("")]
    [AllowAnonymous]
    [HttpGet]
    public DateTimeOffset Get()
    {
        return DateTimeOffset.UtcNow;
    }
}