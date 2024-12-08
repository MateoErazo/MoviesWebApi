﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Services;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movies")]
  public class MoviesController:ControllerBase
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IFileStorer fileStorer;
    private readonly string containerName = "movies";

    public MoviesController(ApplicationDbContext dbContext, IMapper mapper,
      IFileStorer fileStorer)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.fileStorer = fileStorer;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDTO>>> GetAll()
    {
      List<Movie> movies = await dbContext.Movies.ToListAsync();
      return mapper.Map < List<MovieDTO>>(movies);
    }

    [HttpGet("{id:int}", Name ="getMovieById")]
    public async Task<ActionResult<MovieDTO>> GetById([FromRoute] int id) {
      Movie movie = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

      if(movie == null)
      {
        return NotFound($"Don't exist a movie with id {id}.");
      }

      return mapper.Map<MovieDTO>(movie);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromForm] MovieCreationDTO movieCreationDTO)
    {
      Movie movie = mapper.Map<Movie>(movieCreationDTO);

      if (movieCreationDTO.Poster != null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          await movieCreationDTO.Poster.CopyToAsync(ms);
          byte[] content = ms.ToArray();
          var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
          movie.Poster = await fileStorer.SaveFileAsync(content: content, extension: extension,
            container: containerName, contentType: movieCreationDTO.Poster.ContentType);
        }
      }

      dbContext.Add(movie);
      await dbContext.SaveChangesAsync();
      MovieDTO movieDTO = mapper.Map<MovieDTO>(movie);
      return CreatedAtRoute("getMovieById", new {id=movieDTO.Id}, movieDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromForm] MovieCreationDTO movieCreationDTO)
    {
      Movie movie = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

      if (movie == null)
      {
        return NotFound($"Don't exist a movie with id {id}. Please check and try again.");
      }

      movie = mapper.Map(movieCreationDTO, movie);

      if (movieCreationDTO.Poster != null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          await movieCreationDTO.Poster.CopyToAsync(ms);
          byte[] content = ms.ToArray();
          var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);
          movie.Poster = await fileStorer.EditFileAsync(content: content, extension: extension,
            container: containerName, path: movie.Poster, contentType: movieCreationDTO.Poster.ContentType);
        }
      }

      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<MoviePatchDTO> patchDocument)
    {
      if (patchDocument == null)
      {
        return BadRequest();
      }

      Movie movieDB = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

      if (movieDB == null)
      {
        return NotFound();
      }

      MoviePatchDTO moviePatchDTO = mapper.Map<MoviePatchDTO>(movieDB);

      patchDocument.ApplyTo(moviePatchDTO, ModelState);

      bool isValidModel = TryValidateModel(moviePatchDTO);

      if (!isValidModel)
      {
        return BadRequest(ModelState);
      }

      mapper.Map(moviePatchDTO, movieDB);

      await dbContext.SaveChangesAsync();

      return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      bool movieExist = await dbContext.Movies.AnyAsync(x => x.Id == id);

      if (!movieExist)
      {
        return NotFound($"Don't exist a movie with id {id}. Please check and try again.");
      }

      dbContext.Remove(new Movie { Id = id });
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
