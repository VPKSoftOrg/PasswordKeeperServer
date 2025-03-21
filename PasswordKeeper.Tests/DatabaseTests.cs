using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;
using PasswordKeeper.DatabaseMigrations;

namespace PasswordKeeper.Tests;

/// <summary>
/// Tests the database functionality.
/// </summary>
public class DatabaseTests
{
    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
    }

    /// <summary>
    /// Tests the database creation via migration and entity framework core.
    /// </summary>
    [Test]
    public async Task UseEfCoreTest()
    {
        DeleteDatabase();
        Program.Main(["-t"]);
        await using var context = GetMemoryContext();
        _ = await context.Users.CountAsync();
        // TODO::More tests
    }
    
    /// <summary>
    /// Creates a new SQLite database context.
    /// </summary>
    /// <returns>The database context.</returns>
    private static Entities GetMemoryContext()
    {
        var options = new DbContextOptionsBuilder<Entities>()
            .UseSqlite($"Data Source=./{Program.DatabaseName}.db")
            .Options;
        
        return new Entities(options);
    } 
    
    /// <summary>
    /// Deletes the database file.
    /// </summary>
    private static void DeleteDatabase()
    {
        var dbFile = $"./{PasswordKeeper.DatabaseMigrations.Program.DatabaseName}.db";
        if (File.Exists(dbFile))
        {
            File.Delete(dbFile);
        }
    }
}