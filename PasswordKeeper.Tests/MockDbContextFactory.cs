using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;

namespace PasswordKeeper.Tests;

/// <summary>
/// A mock database context factory.
/// </summary>
/// <param name="testClassName">The name of the test class.</param>
/// <seealso cref="IDbContextFactory{Entities}" />
public class MockDbContextFactory(string testClassName) : IDbContextFactory<Entities>
{
    /// <summary>
    /// Creates a new SQLite database context.
    /// </summary>
    /// <returns>The database context.</returns>
    public Entities CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<Entities>()
            .UseSqlite($"Data Source=./{testClassName}.db")
            .Options;
        
        return new Entities(options);
    }
}