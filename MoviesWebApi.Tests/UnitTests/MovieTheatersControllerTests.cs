using MoviesWebApi.Controllers;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests.UnitTests
{
  [TestClass]
  public class MovieTheatersControllerTests:TestsBase
  {
    [TestMethod]
    public async Task GetMovieTheatersIn5KmOrLess()
    {
      //preparation
      var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

      using (var dbContext = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
      {
        var movieTheaters = new List<MovieTheater>() 
        {
          new MovieTheater(){Name = "C.C Primavera", Location= geometryFactory.CreatePoint(new Coordinate(-76.510963,3.420064))},
          new MovieTheater(){Name = "C.C La Estación", Location= geometryFactory.CreatePoint(new Coordinate(-76.51383,3.465246))},
          new MovieTheater(){Name = "C.C Jardín Plaza", Location= geometryFactory.CreatePoint(new Coordinate(-76.527593,3.368984))},
          new MovieTheater(){Name = "C.C Chipichape", Location= geometryFactory.CreatePoint(new Coordinate(-76.527813,3.476123))}
        };

        dbContext.AddRange(movieTheaters);
        await dbContext.SaveChangesAsync();
      }

      var filter = new MovieTheaterFilterNearDTO()
      {
        DistanceInKm = 5,
        Longitude = -76.501122,
        Latitude = 3.417154,
      };

      using (var dbContext = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
      {
        var mapper = ConfigAutoMapper();
        var controller = new MovieTheatersController(dbContext,mapper,geometryFactory);
        var response = await controller.GetNearby(filter);
        var result = response.Value;
        Assert.AreEqual(1, result.Count);
      }
    }
  }
}
