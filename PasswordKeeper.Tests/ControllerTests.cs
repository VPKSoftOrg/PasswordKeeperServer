using Microsoft.AspNetCore.Mvc;
using PasswordKeeper.Classes;
using PasswordKeeper.DatabaseMigrations;
using PasswordKeeper.DTO;
using PasswordKeeper.Interfaces.Enumerations;
using PasswordKeeper.Server.Controllers;
using PasswordKeeper.Server.Controllers.Extensions;

namespace PasswordKeeper.Tests;

/// <summary>
/// Tests the api controllers.
/// </summary>
public class ControllerTests
{
    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        Helpers.DeleteDatabase(nameof(ControllerTests));
        Program.Main([$"-t {nameof(ControllerTests)}",]);
        Server.Program.GetJwtKey = Helpers.GetJwtKey;
        ControllerExtensions.GetLoggedUserIdFunc = @base => 1;
        ControllerExtensions.GetLoggedUserNameFunc = @base => "firsUserIsAdmin";
    }
    
    /// <summary>
    /// Tests the AuthenticationController.Login action.
    /// </summary>
    [Test]
    public async Task AuthenticationControllerLoginTest()
    {
        var dbContextFactory = Helpers.GetMockDbContextFactory(nameof(ControllerTests));
        var dataAccess = new PasswordKeeper.DataAccess.Users(dbContextFactory, Helpers.CreateMapper());
        var businessLogic = new PasswordKeeper.BusinessLogic.Users(dataAccess);
        var controller = new AuthenticationController(businessLogic);
        var loginData = new AuthenticationController.UserLogin("firsUserIsAdmin", "password");
        var result = await controller.Login(loginData);
        
        // The first request should fail as the password does not meet the complexity requirements
        var value = ((BadRequestObjectResult)result).Value!.ToString();
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        Assert.That(value, Is.EqualTo(Passwords.CreateMessageString(LoginRejectReason.PasswordMustContainAtLeastOneUppercaseLetter)));
        
        // The second one should succeed as the password meets the complexity requirements
        loginData = new AuthenticationController.UserLogin("firsUserIsAdmin", "Pa1sword%");
        result = await controller.Login(loginData);
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        await dbContextFactory.DisposeAsync();
    }
    
    /// <summary>
    /// Tests the AuthenticationController.CreateUser action.
    /// </summary>
    [Test]
    public async Task ControllerCreateUserTest()
    {
        var dbContextFactory = Helpers.GetMockDbContextFactory(nameof(ControllerTests));
        var dataAccess = new PasswordKeeper.DataAccess.Users(dbContextFactory, Helpers.CreateMapper());
        var businessLogic = new PasswordKeeper.BusinessLogic.Users(dataAccess);
        var authenticationController = new AuthenticationController(businessLogic);
        var usersController = new UsersController(businessLogic);
        var loginData = new AuthenticationController.UserLogin("firsUserIsAdmin", "Pa1sword%");
        await authenticationController.Login(loginData);
        
        var user = new UsersController.UserChangeRequest(0, "normalUser", "pAssw0rd_");

        var createdUser = await usersController.CreateUser(user);
        
        Assert.That(createdUser, Is.TypeOf<OkObjectResult>());
        var userDto = ((OkObjectResult)createdUser).Value as UserDto;
        Assert.That(userDto, Is.TypeOf<UserDto>());
        Assert.That(userDto.Username, Is.EqualTo("normalUser"));
        await dbContextFactory.DisposeAsync();
    }
}