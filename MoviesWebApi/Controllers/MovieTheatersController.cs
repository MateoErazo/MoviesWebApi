using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movietheaters")]
  public class MovieTheatersController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public MovieTheatersController(ApplicationDbContext dbContext, IMapper mapper)
      :base(dbContext:dbContext, mapper:mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
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
