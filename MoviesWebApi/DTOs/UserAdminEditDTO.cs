using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class UserAdminEditDTO
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
