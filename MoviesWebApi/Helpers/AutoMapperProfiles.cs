using AutoMapper;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Helpers
{
  public class AutoMapperProfiles:Profile
  {
    public AutoMapperProfiles() {
      CreateMap<Gender, GenderDTO>().ReverseMap();
      CreateMap<GenderCreationDTO, Gender>();

      CreateMap<Actor, ActorDTO>().ReverseMap();
      CreateMap<ActorCreationDTO, Actor>()
        .ForMember(x => x.Picture, options => options.Ignore());
      CreateMap<ActorPatchDTO, Actor>().ReverseMap();

      CreateMap<Movie, MovieDTO>().ReverseMap();
      CreateMap<MovieCreationDTO, Movie>()
        .ForMember(x => x.Poster, options => options.Ignore())
        .ForMember(x => x.MovieActors, options => options.MapFrom(MovieCreationDtoActorsMap))
        .ForMember(x => x.MovieGenders, options => options.MapFrom(MovieCreationDtoGendersMap));
      CreateMap<MoviePatchDTO, Movie>().ReverseMap();
    }

    private List<MovieActor> MovieCreationDtoActorsMap(MovieCreationDTO movieCreationDTO, Movie movie)
    {
      List<MovieActor> result = new List<MovieActor>();

      if (movieCreationDTO.Actors == null) { return result; }

      foreach (MovieActorCreationDTO actor in movieCreationDTO.Actors)
      {
        result.Add(new MovieActor
        {
          ActorId = actor.ActorId,
          CharacterName = actor.CharacterName
        });
      }

      return result;
    }

    private List<MovieGender> MovieCreationDtoGendersMap(MovieCreationDTO movieCreationDTO, Movie movie)
    {
      List<MovieGender> result = new List<MovieGender>();

      if (movieCreationDTO.GendersIds == null) { return result; }

      foreach (int genderId in movieCreationDTO.GendersIds)
      {
        result.Add(new MovieGender
        {
          GenderId = genderId
        });
      }

      return result;
    }
  }
}
