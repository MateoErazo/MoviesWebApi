using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace MoviesWebApi.Filters
{
  public class MovieExistAttribute : Attribute, IAsyncResourceFilter
  {
    private readonly ApplicationDbContext dbContext;

    public MovieExistAttribute(ApplicationDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
      var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
      if (movieIdObject == null) { return; }

      int movieId = int.Parse(movieIdObject.ToString());

      bool movieExist = await dbContext.Movies.AnyAsync(x => x.Id == movieId);
      if (!movieExist)
      {
        context.Result = new NotFoundResult();
      }
      else
      {
        await next();
      }
    }
  }
}
