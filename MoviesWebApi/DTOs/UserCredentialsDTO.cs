using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class UserCredentialsDTO
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
