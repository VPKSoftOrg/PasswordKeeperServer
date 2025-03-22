using Microsoft.AspNetCore.Mvc;
using PasswordKeeper.Classes;
using PasswordKeeper.DatabaseMigrations;
using PasswordKeeper.Interfaces.Enumerations;
using PasswordKeeper.Server.Controllers;

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
    }
    
    /// <summary>
    /// Tests the authentication controller.
    /// </summary>
    [Test]
    public async Task AuthenticationControllerTest()
    {
        var dataAccess = new PasswordKeeper.DataAccess.Users(Helpers.GetMockDbContextFactory(nameof(ControllerTests)), Helpers.CreateMapper());
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
    }
}