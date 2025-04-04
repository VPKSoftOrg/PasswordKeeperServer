using PasswordKeeper.Classes;
using PasswordKeeper.DataAccess;
using PasswordKeeper.DTO;
using PasswordKeeper.Interfaces.Enumerations;

namespace PasswordKeeper.BusinessLogic;

/// <summary>
/// User business logic class.
/// </summary>
public class UsersBusinessLogic(PasswordKeeper.DataAccess.UsersDataAccess usersDataAccess)
{
    /// <inheritdoc cref="UsersDataAccess.UpsertUser"/>
    public async Task<UserDto?> UpsertUser(UserDto userDto)
    {
        return await usersDataAccess.UpsertUser(userDto);
    }
    
    /// <inheritdoc cref="UsersDataAccess.GetUserByName"/>
    public async Task<UserDto?> GetUserByName(string name)
    {
        return await usersDataAccess.GetUserByName(name);
    }
    
    /// <inheritdoc cref="UsersDataAccess.GetUserById"/>
    public async Task<UserDto?> GetUserById(long id)
    {
        return await usersDataAccess.GetUserById(id);
    }

    /// <inheritdoc cref="UsersDataAccess.UsersExist"/>    
    public async Task<bool> UsersExist(bool? admin = null)
    {
        return await usersDataAccess.UsersExist(admin);
    }

    /// <inheritdoc cref="UsersDataAccess.GetAllUsers"/>
    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        return await usersDataAccess.GetAllUsers();
    }
    
    /// <inheritdoc cref="UsersDataAccess.DeleteUser"/>
    public async Task DeleteUser(long id)
    {
        await usersDataAccess.DeleteUser(id);
    }
    
    private const int IterationCount = 1000000;

    /// <summary>
    /// Hashes a password using the PBKDF2 algorithm with HMACSHA512.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use. If <c>null</c>, a random salt is generated.</param>
    /// <returns>The hashed password as a base-64 encoded string.</returns>
    public static string HashPassword(string password, ref byte[]? salt)
    {
        var hash = KeyUtilities.HashKey(password, IterationCount, ref salt);
        return hash;
    }

    /// <summary>
    /// Verifies if the provided password, when hashed with the given salt, matches the hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The expected hashed password for comparison.</param>
    /// <param name="salt">The salt used during the hashing process.</param>
    /// <returns><c>true</c> if the hashed password matches; otherwise, <c>false</c>.</returns>
    public static bool VerifyPassword(string password, string hashedPassword, byte[] salt)
    {
        var tempSalt = new byte[salt.Length];
        Array.Copy(salt, tempSalt, salt.Length);
        var verifyHash = HashPassword(password, ref tempSalt);

        return verifyHash == hashedPassword;
    }
    
    /// <summary>
    /// Login result.
    /// </summary>
    /// <param name="Success">Whether the login was successful.</param>
    /// <param name="Message">The message associated with the login result.</param>
    /// <param name="Unauthorized">Whether the login was unauthorized.</param>
    /// <param name="Reason">The reason for the login rejection.</param>
    public record LoginResult(bool Success, string Message, bool Unauthorized, LoginRejectReason Reason);
    
    /// <summary>
    /// Logs in the user and returns a JWT token.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="jwtKey">The JWT key.</param>
    /// <param name="pseudoDomain">The pseudo domain.</param>
    /// <returns>A JWT token if the login is valid, otherwise an error message.</returns>
    public async Task<LoginResult> Login(string username, string password, byte[] jwtKey, string pseudoDomain)
    {
        var adminExists = await usersDataAccess.UsersExist(true);
        UserDto? userDto = null;

        if (username.Length < 4)
        {
            return new LoginResult(false, Passwords.CreateMessageString(LoginRejectReason.UsernameMustBeAtLeast4Characters), false, LoginRejectReason.UsernameMustBeAtLeast4Characters);
        }

        if (!Passwords.IsPasswordOk(password, out _, out var reason))
        {
            return new LoginResult(false, Passwords.CreateMessageString(reason), false, reason);
        }
        
        // If no admin user exists, create one if the username is long enough and the password is valid
        if (!adminExists)
        {
            byte []? salt = null;
            userDto = new UserDto
            {
                Username = username,
                PasswordHash = HashPassword(password, ref salt),
                PasswordSalt = Convert.ToBase64String(salt!),
                IsAdmin = true,
                UserFullName = "Administrator",
            };

            userDto = await usersDataAccess.UpsertUser(userDto);

            if (userDto is null)
            {
                return new LoginResult(false, Passwords.CreateMessageString(LoginRejectReason.FailedToCreateAdminUser), false, LoginRejectReason.FailedToCreateAdminUser);
            }
            
            var token = Passwords.GenerateJwtToken(username, userDto.Id, jwtKey, pseudoDomain, userDto.IsAdmin);
            return new LoginResult(true, token, false, LoginRejectReason.None);
        }
        else
        {
            userDto = await usersDataAccess.GetUserByName(username);
        }

        // An existing user, verify the password
        if (userDto is not null)
        {
            if (UsersBusinessLogic.VerifyPassword(password, userDto.PasswordHash,
                    Convert.FromBase64String(userDto.PasswordSalt)))
            {
                var token = Passwords.GenerateJwtToken(username, userDto.Id, jwtKey, pseudoDomain, userDto.IsAdmin);
                return new LoginResult(true, token, false, LoginRejectReason.None);
            }
        }

        return new LoginResult(false, Passwords.CreateMessageString(LoginRejectReason.InvalidUsernameOrPassword), true, LoginRejectReason.InvalidUsernameOrPassword);
    }
}