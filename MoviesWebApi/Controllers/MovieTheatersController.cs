using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using NetTopologySuite.Geometries;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movietheaters")]
  public class MovieTheatersController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly GeometryFactory geometryFactory;

    public MovieTheatersController(ApplicationDbContext dbContext, IMapper mapper, 
      GeometryFactory geometryFactory)
      :base(dbContext:dbContext, mapper:mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.geometryFactory = geometryFactory;
    }

    [HttpGet]
    public async Task<List<MovieTheaterDTO>> GetAll()
    {
      return await Get<MovieTheater, MovieTheaterDTO>();
    }

    [HttpGet("{id:int}",Name ="getMovieTheaterById")]
    public async Task<ActionResult<MovieTheaterDTO>> GetById([FromRoute] int id)
    {
      return await GetById<MovieTheater,MovieTheaterDTO>(id);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<List<MovieTheaterNearResponseDTO>>> GetNearby(
      [FromQuery] MovieTheaterFilterNearDTO movieTheaterDTO)
    {
      Point userLocation = geometryFactory.CreatePoint(new Coordinate(movieTheaterDTO.Longitude, movieTheaterDTO.Latitude));

      var movieTheaters = await dbContext.MovieTheaters
        .OrderBy(x => x.Location.Distance(userLocation))
        .Where(x => x.Location.IsWithinDistance(userLocation, movieTheaterDTO.DistanceInKm * 1000))
        .Select(x => new MovieTheaterNearResponseDTO
        {
          Id = x.Id,
          Name = x.Name,
          DistanceInKm = Math.Round(x.Location.Distance(userLocation)/1000,2)
        })
        .ToListAsync();

      return movieTheaters;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody]MovieTheaterCreationDTO movieTheaterCreationDTO)
    {
      return await Create<MovieTheaterCreationDTO, MovieTheater, MovieTheaterDTO>(movieTheaterCreationDTO, "getMovieTheaterById");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] MovieTheaterCreationDTO movieTheaterCreationDTO)
    {
      return await Update<MovieTheaterCreationDTO, MovieTheater>(id,movieTheaterCreationDTO);
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
      return await Delete<MovieTheater>(id);
    }
  }
}
