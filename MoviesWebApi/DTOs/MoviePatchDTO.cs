using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class MoviePatchDTO
  {
    [StringLength(100)]
    public string Title { get; set; }
    [StringLength(500)]
    public string Description { get; set; }
    public bool InCinemas { get; set; }
    public DateTime ReleaseDate { get; set; }
  }
}
