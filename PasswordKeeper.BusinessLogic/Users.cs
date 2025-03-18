using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PasswordKeeper.DTO;

namespace PasswordKeeper.BusinessLogic;

/// <summary>
/// User business logic class.
/// </summary>
public class Users(PasswordKeeper.DataAccess.Users users)
{
    /// <inheritdoc cref="PasswordKeeper.DataAccess.Users.UpsertUser"/>
    public async Task<bool> UpsertUser(UserDto userDto)
    {
        return await users.UpsertUser(userDto);
    }
    
    /// <inheritdoc cref="PasswordKeeper.DataAccess.Users.GetUserByName"/>
    public async Task<UserDto?> GetUserByName(string name)
    {
        return await users.GetUserByName(name);
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
        if (salt == null)
        {
            RandomNumberGenerator.Create().GetBytes(salt = new byte[32]);    
        }
        
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: IterationCount,
            numBytesRequested: 64);
        return Convert.ToBase64String(hash);
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
}