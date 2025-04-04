using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PasswordKeeper.Classes;

/// <summary>
/// Key utilities.
/// </summary>
public static class KeyUtilities
{
    private const string AlphabetAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Generates a random key of the specified length.
    /// </summary>
    /// <param name="keyLength">The length of the key to generate.</param>
    /// <returns>A random key of the specified length rounded up to the nearest multiple of 6 separated by dashes.</returns>
    public static string CreateRandomKey(int keyLength)
    {
        while (keyLength < 1 && (keyLength % 6) != 0)
        {
            keyLength++;
        }

        var random = new Random();
        var key = new StringBuilder(keyLength + keyLength / 6 - 1);

        for (var i = 0; i < keyLength; i += 6)
        {
            for (var j = 0; j < 6; j++)
            {
                key.Append(AlphabetAndNumbers[random.Next(AlphabetAndNumbers.Length)]);
            }

            key.Append('-');
        }

        return key.ToString()[..(key.Length - 1)];
    }
    
    /// <summary>
    /// Hashes a key using the PBKDF2 algorithm with HMACSHA512.
    /// </summary>
    /// <param name="key">The key to hash.</param>
    /// <param name="iterationCount">The iteration count to use.</param>
    /// <param name="salt">The salt to use. If <c>null</c>, a random salt is generated.</param>
    /// <returns>The hashed password as a base-64 encoded string.</returns>
    public static string HashKey(string key, int iterationCount, ref byte[]? salt)
    {
        if (salt == null)
        {
            RandomNumberGenerator.Create().GetBytes(salt = new byte[32]);    
        }
        
        var hash = KeyDerivation.Pbkdf2(
            password: key,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: iterationCount,
            numBytesRequested: 64);
        return Convert.ToBase64String(hash);
    }
}