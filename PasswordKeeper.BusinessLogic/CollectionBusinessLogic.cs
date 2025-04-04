using System.Security.Cryptography;
using PasswordKeeper.Classes;
using PasswordKeeper.DataAccess;
using PasswordKeeper.DTO;

namespace PasswordKeeper.BusinessLogic;

/// <summary>
/// A business logic for secret store collections related operations.
/// </summary>
public class CollectionBusinessLogic(CollectionsDataAccess collectionsDataAccess)
{
    /// <summary>
    /// Creates a default collection for the user if it doesn't exist.
    /// </summary>
    /// <param name="userId">The ID of the user to create the default collection for.</param>
    /// <returns>A Task&lt;CollectionDto?&gt; representing the asynchronous operation. </returns>
    /// <remarks>If a new collection is created, the access key is also returned. Remember to save
    /// the access key as it is not stored anywhere.</remarks>
    public async Task<CollectionDto?> CreateUserDefaultCollection(long userId)
    {
        var userCollection = await collectionsDataAccess.GetUserDefaultCollection(userId);

        if (userCollection is null)
        {
            byte []? salt;
            RandomNumberGenerator.Create().GetBytes(salt = new byte[32]);
            var key = KeyUtilities.CreateRandomKey(36);
            var keyHash = PasswordKeeper.BusinessLogic.UsersBusinessLogic.HashPassword(key, ref salt);

            userCollection = await collectionsDataAccess.CreateUserDefaultCollection(userId, salt!, keyHash);
            userCollection!.AccessKey = key;
        }

        return userCollection;
    }
}