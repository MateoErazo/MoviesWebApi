using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Helpers;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests
{
  public class TestsBase
  {
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
  }
}
