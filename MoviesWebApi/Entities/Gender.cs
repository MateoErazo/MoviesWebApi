using MoviesWebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
  public class Gender:IId
  {
    public int Id { get; set; }
    [Required]
    [StringLength(40)]
    public string Name { get; set; }
    public List<MovieGender> GenderMovies { get; set; }
  }
}
