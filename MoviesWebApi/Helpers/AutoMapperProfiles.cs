using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using NetTopologySuite.Geometries;

namespace MoviesWebApi.Helpers
{
  public class AutoMapperProfiles:Profile
  {
    public AutoMapperProfiles(GeometryFactory geometryFactory) {
      CreateMap<IdentityUser, UserDTO>();
      CreateMap<MovieTheater, MovieTheaterDTO>()
        .ForMember(x => x.Latitude, x=> x.MapFrom(y => y.Location.Y))
        .ForMember(x => x.Longitude, x=> x.MapFrom(y => y.Location.X));

      CreateMap<MovieTheaterDTO, MovieTheater>()
        .ForMember(x => x.Location, x => 
         x.MapFrom(y => geometryFactory
          .CreatePoint(
            new Coordinate(y.Longitude, y.Latitude))
          ));

      CreateMap<MovieTheaterCreationDTO, MovieTheater>()
        .ForMember(x => x.Location, x =>
         x.MapFrom(y => geometryFactory
          .CreatePoint(
            new Coordinate(y.Longitude, y.Latitude))
          )); ;

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

      CreateMap<Movie, MovieWithCompleteDetailDTO>()
        .ForMember(x => x.Actors, options => options.MapFrom(MovieActorsMap))
        .ForMember(x => x.Genders, options => options.MapFrom(MovieGendersMap));
    }

    private List<GenderDTO> MovieGendersMap(Movie movie, MovieWithCompleteDetailDTO movieWithCompleteDetailDTO)
    {
      List<GenderDTO> result = new List<GenderDTO>();
      if (movie.MovieGenders == null) { return result; };
      foreach (MovieGender movieGender in movie.MovieGenders)
      {
        result.Add(new GenderDTO
        {
          Id = movieGender.GenderId,
          Name = movieGender.Gender.Name
        });
      }
      return result;
    }

    private List<ActorMovieDTO> MovieActorsMap(Movie movie, MovieWithCompleteDetailDTO movieWithCompleteDetailDTO)
    {
      List<ActorMovieDTO> result = new List<ActorMovieDTO> ();
      if (movie.MovieActors == null) { return result; }
      foreach(MovieActor movieActor in movie.MovieActors)
      {
        result.Add(new ActorMovieDTO
        {
          Id = movieActor.ActorId,
          CharacterName = movieActor.CharacterName,
          PersonName = movieActor.Actor.Name
        });
      }
      return result;
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
