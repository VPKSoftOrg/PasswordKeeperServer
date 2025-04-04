using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PasswordKeeper.DAO;
using PasswordKeeper.DTO;

namespace PasswordKeeper.DataAccess;

/// <summary>
/// Collection data access class.
/// </summary>
/// <param name="dbContextFactory">The <c>Entities</c> database context factory.</param>
/// <param name="mapper">An instance to a class implementing the <see cref="IMapper"/> interface.</param>
public class CollectionsDataAccess(IDbContextFactory<Entities> dbContextFactory, IMapper mapper, UsersDataAccess usersDataAccess)
{
    /// <summary>
    /// Creates a default collection for the user if it doesn't exist.
    /// </summary>
    /// <param name="userId">The ID of the user to create the default collection for.</param>
    /// <param name="salt">The salt used to hash the key.</param>
    /// <param name="keyHash">The hashed key.</param>
    /// <returns>A Task&lt;CollectionDto?&gt; representing the asynchronous operation. </returns>
    /// <returns>An asynchronous operation that returns a CollectionDto or null if the user does not exist.</returns>
    public async Task<CollectionDto?> CreateUserDefaultCollection(long userId, byte[] salt, string keyHash)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null)
        {
            return null;
        }

        // Check if the user already has a default collection
        var collectionMember = await context.UserCollectionMembers.Include(f => f.Collection)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.IsDefaultForUser);
        
        // If the user doesn't have a default collection, create one
        if (collectionMember is null)
        {
            var collection = new Collection
            {
                Name = $"Default: {user.UserFullName}",
                ChallengeKeyHash = keyHash,
                ChallengeKeySalt = Convert.ToBase64String(salt),
            }; 
            
            context.Collections.Add(collection);

            await context.SaveChangesAsync();

            // Add user as default collection member for the new collection
            collectionMember = new UserCollectionMember
            {
                IsDefaultForUser = true, 
                UserId = userId, 
                CollectionId = collection.Id,
            };

            context.UserCollectionMembers.Add(collectionMember);
            
            await context.SaveChangesAsync();
            
            return mapper.Map<CollectionDto>(collection);
        }
        
        // If the user already has a default collection, return it
        return mapper.Map<CollectionDto>(collectionMember.Collection);
    }
    
    /// <summary>
    /// Gets the default collection for the user.
    /// </summary>
    /// <param name="userId">The ID of the user to get the default collection for.</param>
    /// <returns>A Task&lt;CollectionDto?&gt; representing the asynchronous operation. </returns>
    public async Task<CollectionDto?> GetUserDefaultCollection(long userId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        
        var collectionMember = await context.UserCollectionMembers.Include(f => f.Collection)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.IsDefaultForUser);
        
        return mapper.Map<CollectionDto?>(collectionMember?.Collection);
    }
}