using MoviesWebApi.Entities;

namespace MoviesWebApi.DTOs
{
  public class MovieIndexDTO
  {
    public List<MovieDTO> ComingSoon { get; set; }
    public List<MovieDTO> InCinemas { get; set; }
  }
}
