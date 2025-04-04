using PasswordKeeper.DAO;

namespace PasswordKeeper.Tests;

/// <summary>
/// Tests the user business logic.
/// </summary>
public class UserBusinessLogicTests
{
    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public async Task Setup()
    {
        DbContextFactory = await Helpers.MakeBasicTestSetup(nameof(UserBusinessLogicTests));
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
    
    /// <summary>
    /// Tests the creation of a collection for a user.
    /// </summary>
    [Test]
    public async Task CreateUserCollectionTest()
    {
        var usersDataAccess = new PasswordKeeper.DataAccess.UsersDataAccess(DbContextFactory, Helpers.CreateMapper());
        var collectionDataAccess =
            new PasswordKeeper.DataAccess.CollectionsDataAccess(DbContextFactory, Helpers.CreateMapper(),
                usersDataAccess);
        // var usersBusinessLogic = new PasswordKeeper.BusinessLogic.UsersBusinessLogic(usersDataAccess);
        var collectionBusinessLogic = new PasswordKeeper.BusinessLogic.CollectionBusinessLogic(collectionDataAccess);

        var result = await collectionBusinessLogic.CreateUserDefaultCollection(1);
        
        Assert.That(result?.AccessKey, Is.Not.Null);
        
        // A collection with no access key should be returned if the collection already exists
        result = await collectionBusinessLogic.CreateUserDefaultCollection(1);
        Assert.That(result?.AccessKey, Is.Null);
    }
}