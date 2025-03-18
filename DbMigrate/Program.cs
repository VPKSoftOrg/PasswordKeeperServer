using System.Text.RegularExpressions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace DbMigrate;

/// <summary>
/// The console application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Entry point for the application. Sets up the database connection and migration services.
    /// </summary>
    /// <param name="args">Command line arguments. Expects the first argument to be the database connection string.</param>
    static void Main(string[] args)
    {
        var connectionString = args[0];
        
        Console.WriteLine(connectionString);

        // Replace the database name to sys, that should always exist in a MariaDB server
        var connectionStringMaster = Regex.Replace(connectionString, "Database=.*?(;|$)", "database=sys;");
        
        // Replace the database name to the software database name also
        // appending the database name to the connection string if it doesn't exist
        if (!Regex.IsMatch(connectionString, "Database=.*?(;|$)"))
        {
            connectionString = connectionString.EndsWith(';') ? $"{connectionString}Database={DatabaseName}" : $"{connectionString};Database={DatabaseName}";
        }
        else
        {
            connectionString = Regex.Replace(connectionString, "Database=.*?(;|$)", $"Database={DatabaseName}$1");
        }
        
        // Create the database, this is done before the migrations are run
        using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionStringMaster);
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.CommandText = $"CREATE DATABASE IF NOT EXISTS {DatabaseName}";
        command.ExecuteNonQuery();
        
        connection.Close();
        
        using var serviceProvider = CreateServices(connectionString);
        using var scope = serviceProvider.CreateScope();

        UpdateDatabase(scope.ServiceProvider);
    }
    
    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static ServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add MariaDB support to FluentMigrator
                .AddMySql5()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }    
    
    public const string DatabaseName = "password_keeper";
}