using MoviesWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Validations
{
  public class TypeFileAttribute: ValidationAttribute
  {
    private readonly string[] acceptedTypeFiles;

    public TypeFileAttribute(string[] acceptedTypeFiles)
    {
      this.acceptedTypeFiles = acceptedTypeFiles;
    }

    public TypeFileAttribute(TypeFileGroup typeFileGroup)
    {
      if (typeFileGroup == TypeFileGroup.Images)
      {
        acceptedTypeFiles = new string[] { "image/jpeg", "image/png", "image/gif"};
      }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value == null)
      {
        return ValidationResult.Success;
      }

      IFormFile formFile = value as IFormFile;

      if (formFile == null)
      {
        return ValidationResult.Success;
      }

      if (!acceptedTypeFiles.Contains(formFile.ContentType))
      {
        return new ValidationResult($"Only the following file types are allowed: {string.Join(", ", acceptedTypeFiles)}.");
      }

      return ValidationResult.Success;
    }

  }
}
