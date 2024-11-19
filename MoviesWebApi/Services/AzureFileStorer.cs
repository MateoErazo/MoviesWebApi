
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesWebApi.Services
{
  public class AzureFileStorer : IFileStorer
  {
    private readonly string connectionString;

    public AzureFileStorer(IConfiguration configuration)
    {
      connectionString = configuration.GetConnectionString("AzureStorage");
    }
    public async Task DeleteFileAsync(string path, string container)
    {
      if (string.IsNullOrEmpty(path))
      {
        return;
      }

      BlobContainerClient client = new BlobContainerClient(connectionString, container);
      await client.CreateIfNotExistsAsync();
      string file = Path.GetFileName(path);
      BlobClient blob = client.GetBlobClient(file);
      await blob.DeleteIfExistsAsync();
    }

    public async Task<string> EditFileAsync(byte[] content, string extension, 
      string container, string path, string contentType)
    {
      await DeleteFileAsync(path, container);
      return await SaveFileAsync(content, extension, container, contentType);
    }

    public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
    {
      BlobContainerClient client = new BlobContainerClient(connectionString, container);
      await client.CreateIfNotExistsAsync();
      client.SetAccessPolicy(PublicAccessType.Blob);

      string nameFile = $"{Guid.NewGuid()}{extension}";
      BlobClient blob = client.GetBlobClient(nameFile);

      BlobUploadOptions blobUploadOptions = new BlobUploadOptions();
      BlobHttpHeaders blobHttpHeader = new BlobHttpHeaders();
      blobHttpHeader.ContentType = contentType;
      blobUploadOptions.HttpHeaders = blobHttpHeader;

      await blob.UploadAsync(new BinaryData(content), blobUploadOptions);
      return blob.Uri.ToString();
    }
  }
}
