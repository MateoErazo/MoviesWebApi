using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using MoviesWebApi.Controllers;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests.UnitTests
{
  [TestClass]
  public class MoviesControllerTests:TestsBase
  {
    private async Task<string> CreateTestData()
    {
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      Gender gender = new Gender() { Name = "Gender 1"};

      var movies = new List<Movie>()
      {
        new Movie(){Title="Movie 1 Old", ReleaseDate = new DateTime(2010,12,30), InCinemas = false},
        new Movie(){Title="Movie 2 coming soon", ReleaseDate = DateTime.Today.AddDays(1), InCinemas = false},
        new Movie(){Title="Movie 3 in cinemas", ReleaseDate = DateTime.Today.AddDays(-1), InCinemas = true}
      };

      Movie movieWithGender = new Movie()
      {
        Title = "Movie 4 with gender",
        ReleaseDate = new DateTime(2010,7,7),
        InCinemas = false
      };

      movies.Add(movieWithGender);

      dbContext.Genders.Add(gender);
      dbContext.Movies.AddRange(movies);
      await dbContext.SaveChangesAsync();

      var movieGender = new MovieGender()
      {
        GenderId = gender.Id,
        MovieId = movieWithGender.Id
      };

      dbContext.MoviesGenders.Add(movieGender);
      await dbContext.SaveChangesAsync();

      return databaseName;
    }

    [TestMethod]
    public async Task FilterMovieByMovieTitle()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext,mapper,null,null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      string movieTitle = "Movie 1 Old";

      var moviesFilterDTO = new MoviesFilterDTO()
      {
        Title = movieTitle,
        PageSize = 10
      };

      //test
      var response = await controller.Filter(moviesFilterDTO);
      var movies = response.Value;

      //verification
      Assert.AreEqual(1, movies.Count);
      Assert.AreEqual(movieTitle, movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMovieByMoviesInCinemas()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext, mapper, null, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      string movieTitle = "Movie 3 in cinemas";

      var moviesFilterDTO = new MoviesFilterDTO()
      {
        InCinemas = true,
        PageSize = 10
      };

      //test
      var response = await controller.Filter(moviesFilterDTO);
      var movies = response.Value;

      //verification
      Assert.AreEqual(1, movies.Count);
      Assert.IsTrue(movies[0].InCinemas);
      Assert.AreEqual(movieTitle, movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMovieByComingSoon()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext, mapper, null, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      string movieTitle = "Movie 2 coming soon";

      var moviesFilterDTO = new MoviesFilterDTO()
      {
        ComingSoon = true,
        PageSize = 10
      };

      //test
      var response = await controller.Filter(moviesFilterDTO);
      var movies = response.Value;

      //verification
      Assert.AreEqual(1, movies.Count);
      Assert.AreEqual(movieTitle, movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMovieByGender()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext, mapper, null, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      string movieTitle = "Movie 4 with gender";
      int genderId = await dbContext.Genders.Select(x => x.Id).FirstAsync();
      
      var moviesFilterDTO = new MoviesFilterDTO()
      {
        GenderId = genderId,
        PageSize = 10
      };

      //test
      var response = await controller.Filter(moviesFilterDTO);
      var movies = response.Value;

      //verification
      Assert.AreEqual(1, movies.Count);
      Assert.AreEqual(movieTitle, movies[0].Title);
    }

    [TestMethod]
    public async Task FilterMoviesOrderByTitleAscending()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext, mapper, null, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();

      var dbContext2 = BuildDbContext(databaseName);
      var moviesDb = await dbContext2.Movies.OrderBy(x => x.Title).ToListAsync();
      var filterMovies = new MoviesFilterDTO()
      {
        OrderField = "title",
        AscendingOrder = true
      };

      //test
      var response = await controller.Filter(filterMovies);
      var moviesController = response.Value;

      //validation
      Assert.AreEqual(moviesDb.Count, moviesController.Count);

      for (int i = 0; i < moviesDb.Count; i++)
      {
        var movieDb = moviesDb[i];
        var movieController = moviesController[i];
        Assert.AreEqual(movieDb.Title, movieController.Title);
      }
    }

    [TestMethod]
    public async Task FilterMoviesOrderByTitleDescending()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new MoviesController(dbContext, mapper, null, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();

      var dbContext2 = BuildDbContext(databaseName);
      var moviesDb = await dbContext2.Movies.OrderByDescending(x => x.Title).ToListAsync();
      var filterMovies = new MoviesFilterDTO()
      {
        OrderField = "title",
        AscendingOrder = false
      };

      //test
      var response = await controller.Filter(filterMovies);
      var moviesController = response.Value;

      //validation
      Assert.AreEqual(moviesDb.Count, moviesController.Count);

      for (int i = 0; i < moviesDb.Count; i++)
      {
        var movieDb = moviesDb[i];
        var movieController = moviesController[i];
        Assert.AreEqual(movieDb.Title, movieController.Title);
      }
    }

    [TestMethod]
    public async Task FilterByWrongFieldToOrderReturnMovies()
    {
      //preparation
      string databaseName = await CreateTestData();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var mock = new Mock<ILogger<MoviesController>>();
      var controller = new MoviesController(dbContext, mapper, null,mock.Object);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();

      var orderField = "abc123";
      var moviesFilter = new MoviesFilterDTO()
      {
        OrderField = orderField,
        AscendingOrder = true
      };

      //test
      var response = await controller.Filter(moviesFilter);
      var moviesController = response.Value;

      //verification
      var dbContext2 = BuildDbContext(databaseName);
      var moviesDb = await dbContext2.Movies.ToListAsync();
      Assert.AreEqual(moviesController.Count, moviesDb.Count);
      Assert.AreEqual(1, mock.Invocations.Count);
    }
  }
}
