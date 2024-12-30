using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using MoviesWebApi.Controllers;
using MoviesWebApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests.UnitTests
{
  [TestClass]
  public class AccountsControllerTests: TestsBase
  {
    [TestMethod]
    public async Task CreateUser()
    {
      //preparation
      var databaseName = Guid.NewGuid().ToString();
      //test
      await CreateUserHelper(databaseName);

      //verification
      var dbContext2 = BuildDbContext(databaseName);
      int usersAmount = await dbContext2.Users.CountAsync();
      Assert.AreEqual(1, usersAmount);
    }

    [TestMethod]
    public async Task UserCannotLogIn()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      await CreateUserHelper(databaseName);
      var controller = BuildAccountsController(databaseName);
      var user = new UserCredentialsDTO(){Email = "testuser@hotmail.com",Password = "wrongPass"};
      
      //test
      var response = await controller.Login(user);

      //verification
      Assert.IsNull(response.Value);
      var result = response.Result as BadRequestObjectResult;
      Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UserCanLogIn()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      await CreateUserHelper(databaseName);
      var controller = BuildAccountsController(databaseName);
      var user = new UserCredentialsDTO() { Email = "testuser@hotmail.com", Password = "Aa123456!" };

      //test
      var response = await controller.Login(user);

      //verification
      Assert.IsNotNull(response.Value);
      Assert.IsNotNull(response.Value.Token);
    }

    private async Task CreateUserHelper(string databaseName)
    {
      var accountsController = BuildAccountsController(databaseName);
      var user = new UserCredentialsDTO()
      {
        Email = "testuser@hotmail.com",
        Password = "Aa123456!"
      };
      await accountsController.CreateUser(user);
    }

    private AccountsController BuildAccountsController(string databaseName)
    {
      var dbContext = BuildDbContext(databaseName);
      var myUserStore = new UserStore<IdentityUser>(dbContext);
      var userManager = BuildUserManager(myUserStore);
      var mapper = ConfigAutoMapper();

      var httpContext = new DefaultHttpContext();
      MockAuth(httpContext);
      var signInManager = SetupSignInManager(userManager, httpContext);

      var myConfig = new Dictionary<string, string>
      {
        {"jwt:key","H8VTD6H484YJ8Y8JDUB4Y984VBJU8KF9K798O7IFVYD8S3C8T48Y97J8YJVJ5J81J7J64J8U24J8G4DTFHCH8YV2985HMB71M2JHGM8MQ"}
      };

      var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(myConfig)
        .Build();

      return new AccountsController(userManager,signInManager,configuration,dbContext,mapper);
    }

    private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
    {
      store = store ?? new Mock<IUserStore<TUser>>().Object;
      var options = new Mock<IOptions<IdentityOptions>>();
      var idOptions = new IdentityOptions();
      idOptions.Lockout.AllowedForNewUsers = false;

      options.Setup(o => o.Value).Returns(idOptions);

      var userValidators = new List<IUserValidator<TUser>>();

      var validator = new Mock<IUserValidator<TUser>>();
      userValidators.Add(validator.Object);
      var pwdValidators = new List<PasswordValidator<TUser>>();
      pwdValidators.Add(new PasswordValidator<TUser>());

      var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
        userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(), null,
        new Mock<ILogger<UserManager<TUser>>>().Object);

      validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
        .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

      return userManager;
    }

    private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
      HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
      IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
    {
      var contextAccesor = new Mock<IHttpContextAccessor>();
      contextAccesor.Setup(a => a.HttpContext).Returns(context);
      identityOptions = identityOptions ?? new IdentityOptions();
      var options = new Mock<IOptions<IdentityOptions>>();
      options.Setup(a => a.Value).Returns(identityOptions);
      var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager,options.Object);
      schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
      var sm = new SignInManager<TUser>(manager,contextAccesor.Object,claimsFactory,options.Object,null,schemeProvider, new DefaultUserConfirmation<TUser>());
      sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
      return sm;
    }

    private Mock<IAuthenticationService> MockAuth(HttpContext httpContext)
    {
      var auth = new Mock<IAuthenticationService>();
      httpContext.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
      return auth;
    }
  }
}
