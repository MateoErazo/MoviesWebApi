﻿using AutoMapper;
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
        .ForMember(x => x.Poster, options => options.Ignore());
      CreateMap<MoviePatchDTO, Movie>().ReverseMap();
    }
  }
}
