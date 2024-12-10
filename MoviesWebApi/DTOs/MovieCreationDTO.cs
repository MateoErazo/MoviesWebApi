using MoviesWebApi.Enums;
using MoviesWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class MovieCreationDTO
  {
    [Required]
    [StringLength(maximumLength:100)]
    public string Title { get; set; }
    [StringLength(500)]
    public string Description { get; set; }
    public bool InCinemas { get; set; }
    public DateTime ReleaseDate { get; set; }
    [FileWeight(maximumFileWeightInMB:4)]
    [TypeFile(TypeFileGroup.Images)]
    public IFormFile Poster { get; set; }
  }
}
