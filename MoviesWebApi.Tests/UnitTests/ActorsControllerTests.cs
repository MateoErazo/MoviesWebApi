using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoviesWebApi.Controllers;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests.UnitTests
{
  [TestClass]
  public class ActorsControllerTests:TestsBase
  {
    [TestMethod]
    public async Task GetPagedActors()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      dbContext.Actors.Add(new Actor() { Name = "Actor 1"});
      dbContext.Actors.Add(new Actor() { Name = "Actor 2" });
      dbContext.Actors.Add(new Actor() { Name = "Actor 3" });
      await dbContext.SaveChangesAsync();

      var dbContext2 = BuildDbContext(databaseName);
      var controller = new ActorsController(dbContext2, mapper, null);
      controller.ControllerContext.HttpContext = new DefaultHttpContext();

      //test
      var resultPageOne = await controller.GetAll(new PaginationDTO() { Page=1, PageSize = 2});
      var actorsPageOne = resultPageOne.Value;

      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      var resultPageTwo = await controller.GetAll(new PaginationDTO() { Page = 2, PageSize = 2 });
      var actorsPageTwo = resultPageTwo.Value;

      controller.ControllerContext.HttpContext = new DefaultHttpContext();
      var resultPageThree = await controller.GetAll(new PaginationDTO() { Page = 3, PageSize = 2 });
      var actorsPageThree = resultPageThree.Value;

      //verification
      Assert.AreEqual(2, actorsPageOne.Count);
      Assert.AreEqual(1, actorsPageTwo.Count);
      Assert.AreEqual(0, actorsPageThree.Count);

    }

    [TestMethod]
    public async Task CreateActorWithoutPicture()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      ActorCreationDTO actorDTO = new ActorCreationDTO() { Name="actor 1", Birthdate = DateTime.Now};
      
      var mock = new Mock<IFileStorer>();
      mock.Setup(x => x.SaveFileAsync(null,null,null,null))
        .Returns(Task.FromResult("url"));

      var controller = new ActorsController(dbContext,mapper,mock.Object);

      //test
      var response = await controller.Create(actorDTO);
      var result = response as CreatedAtRouteResult;

      //verification 1
      Assert.AreEqual(201, result.StatusCode);

      //verification 2
      var dbContext2 = BuildDbContext(databaseName);
      var actors = await dbContext2.Actors.ToListAsync();
      Assert.AreEqual(1, actors.Count);

      //verification 3
      var actor = actors[0];
      Assert.IsNull(actor.Picture);

      //verification 4
      Assert.AreEqual(0,mock.Invocations.Count);
    }

    [TestMethod]
    public async Task CreateActorWithPicture()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      var content = Encoding.UTF8.GetBytes("Test picture");
      var file = new FormFile(new MemoryStream(content),0,content.Length,"Data","picture.jpg");
      file.Headers = new HeaderDictionary();
      file.ContentType = "image/jpg";

      var actorDTO = new ActorCreationDTO()
      {
        Name = "Actor 1",
        Birthdate = DateTime.Now,
        Picture = file
      };

      var mock = new Mock<IFileStorer>();
      mock.Setup(x => x.SaveFileAsync(content,".jpg","actors",file.ContentType))
        .Returns(Task.FromResult("url"));

      var controller = new ActorsController(dbContext,mapper,mock.Object);

      //test
      var response = await controller.Create(actorDTO);
      var result = response as CreatedAtRouteResult;

      //verification
      Assert.AreEqual(201, result.StatusCode);

      var dbContext2 = BuildDbContext(databaseName);
      var actors = await dbContext2.Actors.ToListAsync();
      Assert.AreEqual(1,actors.Count);

      var actor = actors[0];
      Assert.AreEqual("url",actor.Picture);

      Assert.AreEqual(1, mock.Invocations.Count);
    }

    [TestMethod]
    public async Task PatchReturn404IfActorNotExist()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();
      var controller = new ActorsController(dbContext,mapper,null);
      var patchDocument = new JsonPatchDocument<ActorPatchDTO>();
      //test
      var response = await controller.Patch(1, patchDocument);
      var result = response as StatusCodeResult;

      //verification
      Assert.AreEqual(404,result.StatusCode);
    }

    [TestMethod]
    public async Task PatchUpdateASingleField()
    {
      //preparation
      string databaseName = Guid.NewGuid().ToString();
      var dbContext = BuildDbContext(databaseName);
      var mapper = ConfigAutoMapper();

      var birthDate = DateTime.Now;
      dbContext.Actors.Add(new Actor()
      {
        Name="Actor 1",
        Birthdate = birthDate
      });

      await dbContext.SaveChangesAsync();
      
      var dbContext2 = BuildDbContext(databaseName);
      var controller = new ActorsController(dbContext2,mapper,null);

      var objectValidator = new Mock<IObjectModelValidator>();
      objectValidator.Setup(x => x.Validate(
        It.IsAny<ActionContext>(),
        It.IsAny<ValidationStateDictionary>(),
        It.IsAny<string>(),
        It.IsAny<object>()));

      controller.ObjectValidator = objectValidator.Object;

      var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
      patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace","/name",null,"Actor 1 patch updated"));

      //test
      var response = await controller.Patch(1, patchDoc);
      var result = response as StatusCodeResult;

      //validation
      Assert.AreEqual(204, result.StatusCode);

      var dbContext3 = BuildDbContext(databaseName);
      var actor = await dbContext3.Actors.FirstAsync();
      Assert.AreEqual("Actor 1 patch updated",actor.Name);
      Assert.AreEqual(birthDate, actor.Birthdate);

    }
  }
}
