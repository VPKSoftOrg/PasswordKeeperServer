using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;
using PasswordKeeper.DTO;

namespace PasswordKeeper.DataAccess;

/// <summary>
/// User data access class.
/// </summary>
/// <param name="dbContextFactory">The <c>Entities</c> database context factory.</param>
/// <param name="mapper">An instance to a class implementing the <see cref="IMapper"/> interface.</param>
public class Users(IDbContextFactory<Entities> dbContextFactory, IMapper mapper)
{
    /// <summary>
    /// Gets the user with the given name, or null if no such user exists.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The user with the given name, or null if it doesn't exist.</returns>
    public async Task<UserDto?> GetUserByName(string name)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == name);

        return mapper.Map<UserDto?>(user);
    }

    /// <summary>
    /// Upserts a user. If the user doesn't exist, inserts it, otherwise updates it.
    /// </summary>
    /// <param name="userDto">The user to upsert.</param>
    /// <returns><c>true</c> if the user was upserted successfully, otherwise <c>false</c>.</returns>
    public async Task<bool> UpsertUser(UserDto userDto)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == userDto.UserName);
        
        if (user == null)
        {
            user = mapper.Map<User>(userDto);
            await context.Users.AddAsync(user);
        } 
        else 
        {
            mapper.Map(userDto, user);    
        }
        
        await context.SaveChangesAsync();
        
        return true;
    }
}