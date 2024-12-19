using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Helpers;
using MoviesWebApi.Services;
using System.Linq.Dynamic.Core;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movies")]
  public class MoviesController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IFileStorer fileStorer;
    private readonly ILogger<MoviesController> logger;
    private readonly string containerName = "movies";

    public MoviesController(ApplicationDbContext dbContext, IMapper mapper,
      IFileStorer fileStorer, ILogger<MoviesController> logger)
      :base(dbContext:dbContext,mapper:mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.fileStorer = fileStorer;
      this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MovieIndexDTO>> GetAll()
    {
      int top = 5;
      DateTime today = new DateTime(2024,1,1);

      List<Movie> comingSoon = await dbContext.Movies
        .Where(x => x.ReleaseDate > today)
        .OrderBy(x => x.ReleaseDate)
        .Take(top)
        .ToListAsync();

      List<Movie> inCinemas = await dbContext.Movies
        .Where(x => x.InCinemas)
        .OrderBy(x => x.ReleaseDate)
        .Take (top)
        .ToListAsync();

      MovieIndexDTO result = new MovieIndexDTO();
      result.ComingSoon = mapper.Map<List<MovieDTO>>(comingSoon);
      result.InCinemas = mapper.Map<List<MovieDTO>>(inCinemas);

      return result;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MoviesFilterDTO moviesFilterDTO)
    {
      IQueryable<Movie> moviesQueryable = dbContext.Movies.AsQueryable();

      if (!string.IsNullOrEmpty(moviesFilterDTO.Title))
      {
        moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(moviesFilterDTO.Title));
      }

      if (moviesFilterDTO.InCinemas)
      {
        moviesQueryable = moviesQueryable.Where(x => x.InCinemas);
      }

      if (moviesFilterDTO.ComingSoon)
      {
        DateTime today = new DateTime(2024,1,1);
        moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
      }

      if (moviesFilterDTO.GenderId != 0)
      {
        moviesQueryable = moviesQueryable
          .Where(x => x.MovieGenders
            .Select(y => y.GenderId)
            .Contains(moviesFilterDTO.GenderId));
      }

      if (!string.IsNullOrEmpty(moviesFilterDTO.OrderField))
      {
        string orderByType = moviesFilterDTO.AscendingOrder ? "ascending" : "descending";
        try
        {
          moviesQueryable = moviesQueryable.OrderBy($"{moviesFilterDTO.OrderField} {orderByType}");
        }
        catch (Exception ex) {
          logger.LogError(ex.Message,ex);
        }
      }

      await HttpContext.InsertParametersPagination(moviesQueryable, moviesFilterDTO.Pagination.PageSize);

      List<Movie> movies = await moviesQueryable.Paginate(moviesFilterDTO.Pagination).ToListAsync();

      return mapper.Map<List<MovieDTO>>(movies);
    }

    [HttpGet("{id:int}", Name ="getMovieById")]
    public async Task<ActionResult<MovieWithCompleteDetailDTO>> GetById([FromRoute] int id) {
      
      Movie movie = await dbContext.Movies
        .Include(x => x.MovieActors).ThenInclude(y => y.Actor)
        .Include(x => x.MovieGenders).ThenInclude(y => y.Gender)
        .FirstOrDefaultAsync(x => x.Id == id);

      if(movie == null)
      {
        return NotFound($"Don't exist a movie with id {id}.");
      }

      movie.MovieActors = movie.MovieActors.OrderBy(x => x.Order).ToList();
      movie.MovieGenders = movie.MovieGenders.OrderBy(x => x.Gender.Name).ToList();

      return mapper.Map<MovieWithCompleteDetailDTO>(movie);
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

      SetMovieActorsOrder(movie);
      dbContext.Add(movie);
      await dbContext.SaveChangesAsync();
      MovieDTO movieDTO = mapper.Map<MovieDTO>(movie);
      return CreatedAtRoute("getMovieById", new { id = movieDTO.Id }, movieDTO);
    }

    private void SetMovieActorsOrder(Movie movie)
    {
      if (movie.MovieActors != null)
      {
        for (int i = 0; i < movie.MovieActors.Count; i++)
        {
          movie.MovieActors[i].Order = i;
        }
      }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromForm] MovieCreationDTO movieCreationDTO)
    {
      Movie movie = await dbContext.Movies
        .Include(x => x.MovieActors)
        .Include(x => x.MovieGenders)
        .FirstOrDefaultAsync(x => x.Id == id);

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

      SetMovieActorsOrder(movie);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<MoviePatchDTO> patchDocument)
    {
      return await Patch<Movie,MoviePatchDTO>(id, patchDocument);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      return await Delete<Movie>(id);
    }
  }
}
