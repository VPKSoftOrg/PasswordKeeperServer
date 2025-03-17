using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PasswordKeeper.DAO;
using PasswordKeeper.DTO;

namespace PasswordKeeper.DataAccess;

public class Users(IDbContextFactory<Entities> dbContextFactory, IMapper mapper)
{
    public async Task<UserDto?> GetUserByName(string name)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == name);

        return mapper.Map<UserDto?>(user);
    }
}