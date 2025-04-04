﻿using System.Text.RegularExpressions;
using FluentMigrator.Runner;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using PasswordKeeper.Classes;

// ReSharper disable MemberCanBePrivate.Global

namespace PasswordKeeper.DatabaseMigrations;


/// <summary>
/// The console application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The exit code of the application.</returns>
    public static int Main(string[] args)
    {
        return CommandLineApplication.Execute<Program>(args);
    }

    /// <summary>
    /// The name of the database.
    /// </summary>
    [Option(template: "-t|--test", Description = "Use the test database")]
    public string? TestDbName { get; } = null;

    /// <summary>
    /// The connection string to use for the database.
    /// </summary>
    [Option(template: "-c|--connection", Description = "Use the test database")]
    public string ConnectionString { get; } = string.Empty;
    
    /// <summary>
    /// A flag to indicate whether to create the database if it doesn't exist.
    /// </summary>
    [Option(template: "-m|--makeDatabase", Description = "Create the database")]
    public bool MakeDatabase { get; } = false;
    
    /// <summary>
    /// The entry point for the application.
    /// </summary>
    /// <remarks>
    /// If the <see cref="TestDbName"/> option is specified, a test database is used and the connection string is
    /// inferred from the current working directory. Otherwise, the connection string is read from the command
    /// line option <see cref="ConnectionString"/>. The database is created if it doesn't exist, and then the
    /// migrations are run.
    /// </remarks>
    // ReSharper disable once UnusedMember.Global (used by CommandLineUtils)
    public void OnExecute()
    {
        string connectionString;
        if (TestDbName != null)
        {
            // NOTE: Pooling=False is required for SQLite for the database file to be released after migrations!
            connectionString = DatabaseUtilities.GetSQLiteConnectionString(TestDbName);
            IsTestDb = true;
        }
        else
        {
            IsTestDb = false;
            connectionString = ConnectionString;

            // Replace the database name to sys, that should always exist in a MariaDB server
            var connectionStringMaster = Regex.Replace(connectionString, "Database=.*?(;|$)", "database=sys;");

            // Replace the database name to the software database name also
            // appending the database name to the connection string if it doesn't exist
            if (!Regex.IsMatch(connectionString, "Database=.*?(;|$)"))
            {
                connectionString = connectionString.EndsWith(';')
                    ? $"{connectionString}Database={DatabaseName}"
                    : $"{connectionString};Database={DatabaseName}";
            }
            else
            {
                connectionString = Regex.Replace(connectionString, "Database=.*?(;|$)", $"Database={DatabaseName}$1");
            }

            // Extract the database name from the connection string
            DatabaseName = Regex.Match(connectionString, "Database=.*?(;|$)").Value.Replace("Database=", "")
                .Replace(";", "");

            if (MakeDatabase)
            {
                // Create the database, this is done before the migrations are run
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionStringMaster);
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = $"CREATE DATABASE IF NOT EXISTS {DatabaseName}";
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        using var serviceProvider = IsTestDb ? CreateTestServices(connectionString) : CreateServices(connectionString);
        using var scope = serviceProvider.CreateScope();

        UpdateDatabase(scope.ServiceProvider);
    }

    internal static bool IsTestDb;
    
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

    private static ServiceProvider CreateTestServices(string connectionString)
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add MariaDB support to FluentMigrator
                .AddSQLite()
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
    
    /// <summary>
    /// The name of the database.
    /// </summary>
    public static string DatabaseName { get; set; } = "password_keeper";
}