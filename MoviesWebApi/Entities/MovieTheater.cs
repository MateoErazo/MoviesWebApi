using MoviesWebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
  public class MovieTheater: IId
  {
    public int Id { get; set; }
    [Required]
    [StringLength(120)]
    public string Name { get; set; }
   
    public List<MovieTheaterMovie> Movies { get; set; }
  }
}
