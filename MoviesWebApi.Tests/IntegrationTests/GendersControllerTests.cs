using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using Newtonsoft.Json;

namespace MoviesWebApi.Tests.IntegrationTests
{
  [TestClass]
  public class GendersControllerTests:TestsBase
  {
    private static readonly string url = "/api/genders";
    [TestMethod]
    public async Task GetAllGendersEmptyList()
    {
      string databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName);

      var client = factory.CreateClient();
      var response = await client.GetAsync(url);

      response.EnsureSuccessStatusCode();

      var genders = JsonConvert
        .DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

      Assert.AreEqual(0, genders.Count);
    }

    [TestMethod]
    public async Task GetAllGenders()
    {
      string databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName);

      var dbContext = BuildDbContext(databaseName);
      dbContext.Genders.Add(new Gender() { Name = "Gender 1"});
      dbContext.Genders.Add(new Gender() { Name = "Gender 2" });
      await dbContext.SaveChangesAsync();

      var client = factory.CreateClient();
      var response = await client.GetAsync(url);

      response.EnsureSuccessStatusCode();

      var genders = JsonConvert
        .DeserializeObject<List<GenderDTO>>(await response.Content.ReadAsStringAsync());

      Assert.AreEqual(2, genders.Count);
    }

    [TestMethod]
    public async Task DeleteGender()
    {
      var databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName);

      var context = BuildDbContext(databaseName);
      context.Genders.Add(new Gender() { Name = "Gender 1"});
      await context.SaveChangesAsync();

      var client = factory.CreateClient();
      var response = await client.DeleteAsync($"{url}/1");
      response.EnsureSuccessStatusCode();

      var dbContext2 = BuildDbContext(databaseName);
      var exist = await dbContext2.Genders.AnyAsync();
      Assert.IsFalse(exist);
    }

    [TestMethod]
    public async Task DeleteGenderReturn401()
    {
      var databaseName = Guid.NewGuid().ToString();
      var factory = BuildWebApplicationFactory(databaseName, ignoreSecurity: false);

      var client = factory.CreateClient();
      var response = await client.DeleteAsync($"{url}/1");
      Assert.AreEqual("Unauthorized", response.ReasonPhrase);
    }
  }
}
