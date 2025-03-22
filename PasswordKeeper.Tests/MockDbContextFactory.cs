using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;

namespace PasswordKeeper.Tests;

/// <summary>
/// A mock database context factory.
/// </summary>
/// <param name="testClassName">The name of the test class.</param>
/// <seealso cref="IDbContextFactory{Entities}" />
public class MockDbContextFactory(string testClassName) : IDisposableContextFactory<Entities>
{
    private Entities? context;
    
    /// <summary>
    /// Creates a new SQLite database context.
    /// </summary>
    /// <returns>The database context.</returns>
    public Entities CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<Entities>()
            .UseSqlite($"Data Source=./{testClassName}.db")
            .Options;
        
        context = new Entities(options);

        return context;
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        context?.Dispose();
        context = null;
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    public async ValueTask DisposeAsync()
    {
        if (context != null)
        {
            await context.DisposeAsync();
            context = null;
        }
    }
}