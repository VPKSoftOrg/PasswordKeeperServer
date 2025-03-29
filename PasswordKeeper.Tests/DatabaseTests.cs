using Microsoft.EntityFrameworkCore;
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
        Helpers.DeleteDatabase(nameof(DatabaseTests));
        Program.Main([$"-t {nameof(DatabaseTests)}",]);
    }

    /// <summary>
    /// Tests the database creation via migration and entity framework core.
    /// </summary>
    [Test]
    public async Task UseEfCoreTest()
    {
        var dbContextFactory = Helpers.GetMockDbContextFactory(nameof(DatabaseTests));
        await using var context = await dbContextFactory.CreateDbContextAsync();
        _ = await context.Users.CountAsync();
        // TODO::More tests
    }

    /// <summary>
    /// Tests the database down migration.
    /// </summary>
    [Test]
    public void MigrateDown()
    {
        Program.Main([$"-t {nameof(DatabaseTests)}", "-d",]);
    }
}