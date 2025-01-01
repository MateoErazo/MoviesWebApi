using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesWebApi.Filters
{
  public class GlobalExceptionAttribute : ExceptionFilterAttribute
  {
    private readonly ILogger<GlobalExceptionAttribute> logger;
    public GlobalExceptionAttribute(ILogger<GlobalExceptionAttribute> logger)
    {
      this.logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
      logger.LogError(context.Exception, context.Exception.Message);
      base.OnException(context);
    }
  }
}
