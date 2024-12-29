using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
  public class GendersControllerTests: TestsBase
  {
    [TestMethod]
    public async Task GetAllGenders()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      dbContext.Genders.Add(new Gender { Name = "Gender 1"});
      dbContext.Genders.Add(new Gender { Name = "Gender 2" });
      await dbContext.SaveChangesAsync();

      var dbContext2 = BuildDbContext(databaseName);

      //test
      var controller = new GendersController(mapper, dbContext2);
      var response = await controller.GetAll();

      //verification
      var genders = response.Value;
      Assert.AreEqual(2,genders.Count);
    }

    [TestMethod]
    public async Task GetGenderByIdNotExist()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new GendersController(mapper,dbContext);

      //test
      var response = await controller.GetById(1);

      //verification
      var result = response.Result as StatusCodeResult;
      Assert.AreEqual(404,result.StatusCode);
    }

    [TestMethod]
    public async Task GetGenderByIdExisting()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      dbContext.Add(new Gender() { Name = "Gender 1"});
      await dbContext.SaveChangesAsync();
      var dbContext2 = BuildDbContext(databaseName);
      var controller = new GendersController(mapper, dbContext2);

      //test
      var response = await controller.GetById(1);

      //verification
      var result = response.Value;
      Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public async Task CreateGender()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new GendersController(mapper, dbContext);
      GenderCreationDTO genderDTO = new GenderCreationDTO() { Name = "Gender 1"};

      //test
      var response = await controller.Create(genderDTO);
      var result = response as CreatedAtRouteResult;

      //verification 1
      Assert.IsNotNull(result);

      //verification 2
      var dbContext2 = BuildDbContext(databaseName);
      var dbRegistersAmount = await dbContext2.Genders.CountAsync();
      Assert.AreEqual(1, dbRegistersAmount);
    }

    [TestMethod]
    public async Task UpdateGender()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      dbContext.Genders.Add(new Gender() { Name = "Gender 1"});
      await dbContext.SaveChangesAsync();

      var dbContext2 = BuildDbContext(databaseName);
      var controller = new GendersController(mapper, dbContext2);

      int genderId = 1;
      GenderCreationDTO genderDTO = new GenderCreationDTO() { Name = "Gender 1 Updated" };
      
      //test
      var response = await controller.Update(genderId, genderDTO);
      var result = response as StatusCodeResult;

      //verification 1
      Assert.AreEqual(204,result.StatusCode);

      //verification 2
      var dbContext3 = BuildDbContext(databaseName);
      bool genderUpdatedExist = await dbContext3.Genders.AnyAsync(x => x.Name == "Gender 1 Updated");
      Assert.IsTrue(genderUpdatedExist);
    }

    [TestMethod]
    public async Task DeleteGenderByIdNotExisting()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new GendersController(mapper, dbContext);

      //test
      var response = await controller.Delete(1);
      var result = response as StatusCodeResult;

      //verification
      Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteGenderById()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      dbContext.Genders.Add(new Gender() { Name = "Gender 1"});
      dbContext.Genders.Add(new Gender() { Name = "Gender 2" });
      await dbContext.SaveChangesAsync();

      var dbContext2 = BuildDbContext(databaseName);
      var controller = new GendersController(mapper, dbContext2);

      //test
      var response = await controller.Delete(1);
      var result = response as StatusCodeResult;

      //verification 1
      Assert.AreEqual(204, result.StatusCode);

      //verification 2
      var dbContext3 = BuildDbContext(databaseName);
      var genders = await dbContext3.Genders.CountAsync();
      Assert.AreEqual(1, genders);
    }
  }
}
