namespace MoviesWebApi.Services
{
  public interface IFileStorer
  {
    Task<string> SaveFileAsync(byte[] content, string extension, string container, 
      string contentType);

    Task<string> EditFileAsync(byte[] content, string extension, string container,
      string path, string contentType);

    Task DeleteFileAsync(string path, string container);
  }
}
