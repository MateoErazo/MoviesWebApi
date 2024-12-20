using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Helpers;
using MoviesWebApi.Services;
using MoviesWebApi.Services.Impl;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesWebApi
{
    public class Startup
  {
    public Startup (IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
      services.AddSingleton(provider =>
        new MapperConfiguration(config =>
        {
          var geometryFactory = provider.GetRequiredService<GeometryFactory>();
          config.AddProfile(new AutoMapperProfiles(geometryFactory));
        }).CreateMapper()
      );
      
      services.AddControllers()
        .AddNewtonsoftJson();

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
          sqlServerOptions => sqlServerOptions.UseNetTopologySuite());
      });

      services.AddAutoMapper(typeof(Startup));

      services.AddTransient<IFileStorer, LocalFileStorer>();
      services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHttpsRedirection();

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

    }
  }
}
