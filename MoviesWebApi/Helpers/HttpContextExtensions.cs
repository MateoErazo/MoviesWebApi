using Microsoft.EntityFrameworkCore;

namespace MoviesWebApi.Helpers
{
  public static class HttpContextExtensions
  {
    public async static Task InsertParametersPagination<T>(this HttpContext httpContext,
      IQueryable<T> queryable, int pageSize)
    {
      double amount = await queryable.CountAsync();
      double pagesAmount = Math.Ceiling(amount / pageSize);
      httpContext.Response.Headers.Add("pagesAmount", pagesAmount.ToString());
    }
  }
}
