using PasswordKeeper.BusinessLogic;

namespace PasswordKeeper.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestPasswordHash()
    {
        byte[]? salt = null;
        var random = new Random();
        var result = true;
        for (var i = 0; i < 100; i++)
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