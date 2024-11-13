using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Validations
{
  public class FileWeightAttribute: ValidationAttribute
  {
    private readonly int maximumFileWeightInMB;

    public FileWeightAttribute(int maximumFileWeightInMB)
    {
      this.maximumFileWeightInMB = maximumFileWeightInMB;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value == null) { 
        return ValidationResult.Success;
      }

      IFormFile formFile = value as IFormFile;

      if (formFile == null) {
        return ValidationResult.Success;
      }

      if(formFile.Length > maximumFileWeightInMB * 1024 * 1024)
      {
        return new ValidationResult($"The file weight cannot be greater than {maximumFileWeightInMB} MB.");
      }

      return ValidationResult.Success;
    }
  }
}
