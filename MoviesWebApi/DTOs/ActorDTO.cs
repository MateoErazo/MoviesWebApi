using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class ActorDTO
  {
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public DateTime Birthdate { get; set; }

    public string Picture { get; set; }
  }
}
