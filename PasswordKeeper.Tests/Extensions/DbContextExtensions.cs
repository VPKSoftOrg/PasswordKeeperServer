using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;
using PasswordKeeper.DTO;

namespace PasswordKeeper.Tests.Extensions;

/// <summary>
/// Some extension methods for the <c>Entities</c> database context for the unit tests.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Populates the database with some test data.
    /// </summary>
    /// <param name="dbContextFactory">The <c>Entities</c> database context factory.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>The database is populated with a single admin user.</remarks>
    public static async Task PopulateDatabase(this IDbContextFactory<Entities> dbContextFactory)
    {
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
}