using Microsoft.EntityFrameworkCore;

namespace PasswordKeeper.Tests;

/// <summary>
/// A disposable database context factory.
/// </summary>
/// <typeparam name="T">The type of the database context.</typeparam>
/// <seealso cref="IDbContextFactory{T}" />
/// <seealso cref="IDisposable" />
/// <seealso cref="IAsyncDisposable" />
public interface IDisposableContextFactory<T> : IDbContextFactory<T>, IDisposable, IAsyncDisposable 
    where T : DbContext;