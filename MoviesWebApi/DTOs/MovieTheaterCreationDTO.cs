using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class MovieTheaterCreationDTO
  {
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
  }
}
