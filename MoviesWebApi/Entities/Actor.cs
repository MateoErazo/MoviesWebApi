using MoviesWebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
  public class Actor : IId
  {
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public DateTime Birthdate { get; set; }

    public string Picture { get; set; }
    public List<MovieActor> ActorMovies { get; set; }
  }
}
