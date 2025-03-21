using PasswordKeeper.BusinessLogic;

namespace PasswordKeeper.Tests;

/// <summary>
/// Tests the password hashing and verification functionality.
/// </summary>
public class PasswordHashTests
{
    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Tests the password hashing and verification functionality.
    /// </summary>
    /// <remarks>
    /// Generates random passwords of varying lengths, hashes them, and verifies
    /// that the hashed passwords match the original passwords using the generated salt.
    /// The test is performed multiple times to ensure consistency and correctness.
    /// </remarks>
    [Test]
    public void TestPasswordHash()
    {
        byte[]? salt = null;
        var random = new Random();
        var result = true;
        for (var i = 0; i < 10; i++)
        {
            var testPassword = GenerateRandomString(random.Next(10, 30));
            var hashed = Users.HashPassword(testPassword, ref salt);
            result &= Users.VerifyPassword(testPassword, hashed, salt!);
            if (!result)
            {
                break;
            }
        }

        if (result)
        {
            Assert.Pass();    
        }
        else
        {
            Assert.Fail();
        }
    }

    private static string GenerateRandomString(int length)
    {
        // ReSharper disable once StringLiteralTypo
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ÅÄÖåäö!@#$%^&*()";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}