namespace MoviesWebApi.DTOs
{
  public class MovieWithCompleteDetailDTO
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool InCinemas { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Poster { get; set; }

    public List<ActorMovieDTO> Actors { get; set; }
    public List<GenderDTO> Genders { get; set; }

  }
}
