using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Helpers;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests
{
  public class TestsBase
  {
    protected readonly string defaultUserId = "5fe5f85a-f08e-41d5-8218-7e6ef94b9db6";
    protected readonly string defaultUserEmail = "defaultuser@hotmail.com";
    protected ApplicationDbContext BuildDbContext(string databaseName)
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName).Options;
      
      var dbContext = new ApplicationDbContext(options);
      return dbContext;
    }

    protected IMapper ConfigAutoMapper()
    {
      var config = new MapperConfiguration(options =>
      {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid:4326);
        options.AddProfile(new AutoMapperProfiles(geometryFactory));
      });

      return config.CreateMapper();
    }

    protected ControllerContext BuildControllerContext()
    {
      var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, defaultUserEmail),
        new Claim(ClaimTypes.Email, defaultUserEmail),
        new Claim(ClaimTypes.NameIdentifier, defaultUserId)
      }));

      return new ControllerContext()
      {
        HttpContext = new DefaultHttpContext() { User = user }
      };
    }

    protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string databaseName, 
      bool ignoreSecurity = true)
    {
      var factory = new WebApplicationFactory<Startup>();
      factory = factory.WithWebHostBuilder(builder =>
      {
        builder.ConfigureTestServices(services =>
        {
          var descriptorDbContext = services.SingleOrDefault(d => 
          d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

          if (descriptorDbContext != null)
          {
            services.Remove(descriptorDbContext);
          }

          services.AddDbContext<ApplicationDbContext>(options => 
          options.UseInMemoryDatabase(databaseName));

          if (ignoreSecurity)
          {
            services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
            services.AddControllers(options =>
            {
              options.Filters.Add(new FakeUserFilter());
            });
          }
        });
      });

      return factory;
    }
  }
}
