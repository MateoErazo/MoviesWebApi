using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Controllers;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Tests.UnitTests
{
  [TestClass]
  public class ReviewsControllerTests: TestsBase
  {
    [TestMethod]
    public async Task UserCannotCreateTwoReviewsForTheSameMovie()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      CreateMovie(databaseName);

      int movieId = await dbContext.Movies.Select(x => x.Id).FirstAsync();
      Review review1 = new Review()
      {
        MovieId = movieId,
        UserId = defaultUserId,
        Comment = "Example review 1",
        Score = 5
      };

      dbContext.Add(review1);
      await dbContext.SaveChangesAsync();

      var dbContext2 = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new ReviewsController(dbContext2,mapper);
      controller.ControllerContext = BuildControllerContext();

      var review2 = new ReviewCreationDTO() { Comment = "example review 2", Score = 4 };

      //test
      var response = await controller.Create(movieId,review2);
      var result = response as IStatusCodeActionResult;

      //verification
      Assert.AreEqual(400, result.StatusCode.Value);
    }

    [TestMethod]
    public async Task CreateReview()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      CreateMovie(databaseName);

      int movieId = await dbContext.Movies.Select(x => x.Id).FirstAsync();
      var dbContext2 = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new ReviewsController(dbContext2,mapper);
      controller.ControllerContext = BuildControllerContext();

      var review = new ReviewCreationDTO() { Comment = "good movie", Score=5 };

      //test
      var response = await controller.Create(movieId,review);
      var result = response as NoContentResult;

      //verification
      Assert.AreEqual(204, result.StatusCode);

      var dbContext3 = BuildDbContext(databaseName);
      var userReview = await dbContext3.Reviews.FirstAsync();
      Assert.AreEqual(defaultUserId, userReview.UserId);
    }

    private async void CreateMovie(string databaseName)
    {
      var dbContext = BuildDbContext(databaseName);
      dbContext.Movies.Add(new Movie() { Title = "Movie 1"});
      await dbContext.SaveChangesAsync();
    }
  }
}
