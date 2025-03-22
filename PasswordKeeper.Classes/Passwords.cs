using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PasswordKeeper.Interfaces.Enumerations;

namespace PasswordKeeper.Classes;

/// <summary>
/// Password-related helper methods.
/// </summary>
public static class Passwords
{
    /// <summary>
    /// Generates a JWT token for the given username.
    /// </summary>
    /// <param name="username">The username to generate the JWT token for.</param>
    /// <param name="userId">The user ID to generate the JWT token for.</param>
    /// <param name="jwtKey">The JWT key to use for signing the token.</param>
    /// <param name="pseudoDomain">The pseudo domain to use for the token issuer and audience.</param>
    /// <returns>The JWT token.</returns>
    public static string GenerateJwtToken(string username, long userId, byte[] jwtKey, string pseudoDomain)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
        };
        
        var key = new SymmetricSecurityKey(jwtKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: pseudoDomain,
            audience: pseudoDomain,
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
    /// <param name="reason">The reason why the password is not valid.</param>
    /// <returns><c>true</c> if the password is valid according to the rules; otherwise, <c>false</c>.</returns>
    public static bool IsPasswordOk(string password, out string message, out LoginRejectReason reason)
    {
        const string specialCharacters = @"!@#$%^&*()_+/\-=[]{}|;:,.<>?~";
        
        if (password.Length < 8)
        {
            message = PasswordMustBeAtLeast8CharactersLong;
            reason = LoginRejectReason.PasswordMustBeAtLeast8Characters;
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            message = PasswordMustContainAtLeastOneUppercaseLetter;
            reason = LoginRejectReason.PasswordMustContainAtLeastOneUppercaseLetter;
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            message = PasswordMustContainAtLeastOneLowercaseLetter;
            reason = LoginRejectReason.PasswordMustContainAtLeastOneLowercaseLetter;
            return false;
        }
        
        if (!password.Any(char.IsDigit))
        {
            message = PasswordMustContainAtLeastOneNumber;
            reason = LoginRejectReason.PasswordMustContainAtLeastOneDigit;
            return false;
        }
        
        if (!password.Any(c => specialCharacters.Contains(c)))
        {
            message = PasswordMustContainAtLeastOneSpecialCharacter;
            reason = LoginRejectReason.PasswordMustContainAtLeastOneSpecialCharacter;
            return false;
        }

        message = string.Empty;
        reason = LoginRejectReason.Unauthorized;
        
        return true;
    }

    /// <summary>
    /// Creates a message string for the given login reject reason.
    /// </summary>
    /// <param name="reason">The login reject reason.</param>
    /// <returns>The message string.</returns>
    public static string CreateMessageString(LoginRejectReason reason)
    {
        return $"{reason}: {LoginRejectReasonMessages[reason]}";
    }
    
    /// <summary>
    /// An error message indicating that the password must be at least 8 characters long.
    /// </summary>
    public const string PasswordMustBeAtLeast8CharactersLong = "Password must be at least 8 characters long.";
    
    /// <summary>
    /// An error message indicating that the password must contain at least one uppercase letter.
    /// </summary>
    public const string PasswordMustContainAtLeastOneUppercaseLetter = "Password must contain at least one uppercase letter.";
    
    /// <summary>
    /// An error message indicating that the password must contain at least one lowercase letter.
    /// </summary>
    public const string PasswordMustContainAtLeastOneLowercaseLetter = "Password must contain at least one lowercase letter.";
    
    /// <summary>
    /// An error message indicating that the password must contain at least one number.
    /// </summary>
    public const string PasswordMustContainAtLeastOneNumber = "Password must contain at least one number.";
    
    /// <summary>
    /// An error message indicating that the password must contain at least one special character.
    /// </summary>
    public const string PasswordMustContainAtLeastOneSpecialCharacter = "Password must contain at least one special character.";
    
    /// <summary>
    /// An error message indicating that the username must be at least 4 characters long.
    /// </summary>
    public const string UsernameMustBeAtLeast4CharactersLong = "Username must be at least 4 characters long.";
    
    /// <summary>
    /// An error message indicating that the login attempt was unauthorized.
    /// </summary>
    public const string Unauthorized = "Unauthorized";
    
    /// <summary>
    /// An error message indicating that the admin user could not be created.
    /// </summary>
    public const string FailedToCreateAdminUser = "Failed to create admin user.";

    /// <summary>
    /// An error message indicating that the username or password is invalid.
    /// </summary>
    public const string InvalidUsernameOrPassword = "Invalid username or password.";
    
    /// <summary>
    /// An error message indicating that the user already exists.
    /// </summary>
    public const string UserAlreadyExists = "User already exists.";

    /// <summary>
    /// A read-only dictionary mapping login reject reasons to error messages.
    /// </summary>
    public static ReadOnlyDictionary<LoginRejectReason, string> LoginRejectReasonMessages { get; } = new(new Dictionary<LoginRejectReason, string>(
        new List<KeyValuePair<LoginRejectReason, string>>(
        [
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.None, string.Empty),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.Unauthorized, Unauthorized),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.PasswordMustBeAtLeast8Characters,
                PasswordMustBeAtLeast8CharactersLong),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.PasswordMustContainAtLeastOneLowercaseLetter,
                PasswordMustContainAtLeastOneLowercaseLetter),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.PasswordMustContainAtLeastOneUppercaseLetter,
                PasswordMustContainAtLeastOneUppercaseLetter),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.PasswordMustContainAtLeastOneDigit,
                PasswordMustContainAtLeastOneNumber),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.PasswordMustContainAtLeastOneSpecialCharacter,
                PasswordMustContainAtLeastOneSpecialCharacter),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.UsernameMustBeAtLeast4Characters,
                UsernameMustBeAtLeast4CharactersLong),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.FailedToCreateAdminUser,
                FailedToCreateAdminUser),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.InvalidUsernameOrPassword,
                InvalidUsernameOrPassword),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.NotFound, string.Empty),
            new KeyValuePair<LoginRejectReason, string>(LoginRejectReason.UserAlreadyExists, UserAlreadyExists),
        ])));
}