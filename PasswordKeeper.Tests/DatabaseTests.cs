using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;
using PasswordKeeper.DataAccess;
using PasswordKeeper.DatabaseMigrations;
using PasswordKeeper.DTO;

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
    public async Task Setup()
    {
        Helpers.DeleteDatabase(nameof(DatabaseTests));
        Program.Main([$"-t {nameof(DatabaseTests)}",]);
        DbContextFactory = Helpers.GetMockDbContextFactory(nameof(DatabaseTests));
        await PopulateDatabase();
    }

    /// <summary>
    /// Tears down the test environment after each test.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        DbContextFactory.Dispose();
    }
    
    /// <summary>
    /// Gets the mock database context factory.
    /// </summary>
    private IDisposableContextFactory<Entities> DbContextFactory { get; set; } = null!;
    
    private async Task PopulateDatabase()
    {
        var dbContextFactory = DbContextFactory;
        var dataAccess = new PasswordKeeper.DataAccess.UsersDataAccess(dbContextFactory, Helpers.CreateMapper());
        var businessLogic = new PasswordKeeper.BusinessLogic.UsersBusinessLogic(dataAccess);

        byte[]? salt = null;
        var password = "Pa1sword%";

        var user = new UserDto
        {
            Username = "Admin",
            PasswordHash =  PasswordKeeper.BusinessLogic.UsersBusinessLogic.HashPassword(password, ref salt),
            PasswordSalt = Convert.ToBase64String(salt!),
            IsAdmin = false,
            UserFullName = "Administrator",
        };

        await businessLogic.UpsertUser(user);
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
    /// Tests the database up migration.
    /// </summary>
    [Test]
    public void MigrateUp()
    {
        // This is all that is needed as the Setup method runs the up migration
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