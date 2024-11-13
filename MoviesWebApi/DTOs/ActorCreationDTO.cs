using MoviesWebApi.Enums;
using MoviesWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class ActorCreationDTO
  {
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public DateTime Birthdate { get; set; }

    [FileWeight(maximumFileWeightInMB:2)]
    [TypeFile(typeFileGroup: TypeFileGroup.Images)]
    public IFormFile Picture { get; set; }
  }
}
