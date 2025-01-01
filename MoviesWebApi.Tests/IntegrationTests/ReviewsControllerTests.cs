using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using Newtonsoft.Json;

namespace MoviesWebApi.Tests.IntegrationTests
{
  [TestClass]
  public class ReviewsControllerTests : TestsBase
  {
    private static readonly string url = "/api/movies/1/reviews";
    [TestMethod]
    public async Task GetReviewsReturns404MovieDoNotExist()
    {
      string databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName);

      var client = factory.CreateClient();
      var response = await client.GetAsync(url);
      Assert.AreEqual(404, (int)response.StatusCode);
    }

    [TestMethod]
    public async Task GetReviewsReturnsEmptyList()
    {
      string databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName);
      var dbContext = BuildDbContext(databaseName);
      dbContext.Movies.Add(new Movie() { Title = "Movie 1"});
      await dbContext.SaveChangesAsync();

      var client = factory.CreateClient();
      var response = await client.GetAsync(url);

      response.EnsureSuccessStatusCode();

      var reviews = JsonConvert.DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());
      Assert.AreEqual(0, reviews.Count());
    }
  }
}
