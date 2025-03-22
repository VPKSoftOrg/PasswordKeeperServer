namespace PasswordKeeper.Classes;

/// <summary>
/// Database utilities.
/// </summary>
public static class DatabaseUtilities
{
    /// <summary>
    /// Gets the connection string for a SQLite database.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <returns>The connection string.</returns>
    // ReSharper disable once InconsistentNaming (this is how SQLite is written)
    public static string GetSQLiteConnectionString(string databaseName)
    {
        return $"Data Source=./{databaseName}.db;Pooling=False;";
    }
}