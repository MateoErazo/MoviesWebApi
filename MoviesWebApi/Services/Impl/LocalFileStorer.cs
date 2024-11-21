
namespace MoviesWebApi.Services.Impl
{
  public class LocalFileStorer : IFileStorer
  {
    private readonly IWebHostEnvironment env;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LocalFileStorer(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) {
      this.env = env;
      this.httpContextAccessor = httpContextAccessor;
    }

    public Task DeleteFileAsync(string path, string container)
    {
      if (path != null) {
        string nameFile = Path.GetFileName(path);
        string directoryFile = Path.Combine(env.WebRootPath, container, nameFile);

        if (File.Exists(directoryFile))
        {
          File.Delete(directoryFile);
        }
      }

      return Task.FromResult(0);
    }

    public async Task<string> EditFileAsync(byte[] content, string extension, string container, string path, string contentType)
    {
      await DeleteFileAsync(path, container);
      return await SaveFileAsync(content, extension, container, contentType);
    }

    public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
    {
      string nameFile = $"{Guid.NewGuid()}{extension}";
      string folder = Path.Combine(env.WebRootPath, container);

      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      string path = Path.Combine(folder, nameFile);
      await File.WriteAllBytesAsync(path, content);

      string actualUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
      string DbUrl = Path.Combine(actualUrl, container, nameFile).Replace("\\","/");
      return DbUrl;

    }
  }
}
