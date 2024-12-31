using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MoviesWebApi.Tests
{
  public class FakeUserFilter : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
      {
        new Claim(ClaimTypes.Email, "example@hotmail.com"),
        new Claim(ClaimTypes.Name, "example@hotmail.com"),
        new Claim(ClaimTypes.NameIdentifier, "e663812d-4944-40cb-a014-3c2f7d4a57bd")
      },"test"));

      await next();
    }
  }
}
