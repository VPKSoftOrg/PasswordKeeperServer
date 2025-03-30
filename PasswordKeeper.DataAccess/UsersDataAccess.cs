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
public class UsersDataAccess(IDbContextFactory<Entities> dbContextFactory, IMapper mapper)
{
    /// <summary>
    /// Gets the user with the given name, or null if no such user exists.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The user with the given name, or null if it doesn't exist.</returns>
    public async Task<UserDto?> GetUserByName(string name)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(user => user.Username == name);

        return mapper.Map<UserDto?>(user);
    }

    /// <summary>
    /// Gets the user with the given ID, or null if no such user exists.
    /// </summary>
    /// <param name="id">The user ID to search for.</param>
    /// <returns>The user with the given ID, or null if it doesn't exist.</returns>
    public async Task<UserDto?> GetUserById(long id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id);
        
        return mapper.Map<UserDto?>(user);
    }


    /// <summary>
    /// Checks if users exist in the database, optionally filtering by admin status.
    /// </summary>
    /// <param name="admin">If <c>null</c>, checks if any users exist. If <c>true</c>, checks if any admin users exist. If <c>false</c>, checks if any non-admin users exist.</param>
    /// <returns><c>true</c> if the specified users exist, <c>false</c> otherwise.</returns>
    public async Task<bool> UsersExist(bool? admin = null)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        if (admin == null)
        {
            return await context.Users.AnyAsync();    
        }
        
        return admin.Value ? await context.Users.AnyAsync(user => user.IsAdmin) : await context.Users.AnyAsync(user => !user.IsAdmin);
    }
    
    /// <summary>
    /// Upserts a user. If the user doesn't exist, inserts it, otherwise updates it.
    /// </summary>
    /// <param name="userDto">The user to upsert.</param>
    /// <returns>The upserted user data or <c>null</c> if the operation failed.</returns>
    public async Task<UserDto?> UpsertUser(UserDto userDto)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var user = await context.Users.FirstOrDefaultAsync(user => user.Username == userDto.Username);
        
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
        
        return mapper.Map<UserDto>(user);
    }
    
    /// <summary>
    /// Deletes the user with the given ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteUser(long id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == id);
        
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        
        // TODO:Delete user data
    }
    
    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        return mapper.Map<IEnumerable<UserDto>>(await context.Users.ToListAsync());
    }
}