using Microsoft.Extensions.Diagnostics.HealthChecks;
using MoviesWebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
  public class Movie:IId
  {
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    [StringLength(500)]
    public string Description { get; set; }
    public bool InCinemas { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Poster { get; set; }

    public List<MovieActor> MovieActors { get; set; }
    public List<MovieGender> MovieGenders { get; set; }
    public List<MovieTheaterMovie> MovieTheaters { get; set; }
  }
}
