using System.Security.Cryptography;
using AutoMapper;
using PasswordKeeper.DAO;
using PasswordKeeper.DataAccess;
using PasswordKeeper.DatabaseMigrations;
using PasswordKeeper.Tests.Extensions;

namespace PasswordKeeper.Tests;

/// <summary>
/// Helper methods for the tests.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Creates a new SQLite database context factory.
    /// </summary>
    /// <param name="testClassName">The name of the test class.</param>
    /// <returns>The database context.</returns>
    public static IDisposableContextFactory<Entities> GetMockDbContextFactory(string testClassName)
    {
        return new MockDbContextFactory(testClassName);
    }

    /// <summary>
    /// Deletes the database file.
    /// </summary>
    /// <param name="testClassName">The name of the test class.</param>
    public static void DeleteDatabase(string testClassName)
    {
        var dbFile = $"./{testClassName}.db";
        if (File.Exists(dbFile))
        {
            File.Delete(dbFile);
        }
    }

    /// <summary>
    /// Creates an instance to a class implementing the <see cref="IMapper"/> interface.
    /// </summary>
    /// <returns>An instance to a class implementing the <see cref="IMapper"/> interface.</returns>
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        return config.CreateMapper();
    }
    
    private static byte[]? _jwtKey;
    
    /// <summary>
    /// Gets the mock JWT key.
    /// </summary>
    /// <returns>The mock JWT key.</returns>
    public static byte[] GetJwtKey()
    {
        if (_jwtKey == null)
        {
            // If the key is not in the database, generate a new one
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            _jwtKey = randomBytes;
        }

        return _jwtKey;
    }

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    /// <param name="testClassName">The name of the test class.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task<IDisposableContextFactory<Entities>> MakeBasicTestSetup(string testClassName)
    {
        Helpers.DeleteDatabase(testClassName);
        Program.Main([$"-t {testClassName}",]);
        var dbContextFactory = Helpers.GetMockDbContextFactory(testClassName);
        await dbContextFactory.PopulateDatabase();
        return dbContextFactory;
    }
}