using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using System.Security.Claims;
namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movies/{movieId:int}/reviews")]
  public class ReviewsController:CustomBaseController
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public ReviewsController(ApplicationDbContext dbContext,IMapper mapper)
      :base(dbContext:dbContext, mapper:mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReviewDTO>>> GetAll(
      int movieId, [FromQuery] PaginationDTO paginationDTO)
    {
      var queryable = dbContext.Reviews.Include(x => x.User).AsQueryable();
      queryable = queryable.Where(x => x.MovieId == movieId);
      return await Get<Review, ReviewDTO>(paginationDTO, queryable);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Create(int movieId,[FromBody] ReviewCreationDTO reviewCreationDTO)
    {
      bool movieExist = await dbContext.Movies.AnyAsync(x => x.Id == movieId);
      if (!movieExist)
      {
        return NotFound();
      }

      string userId = HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

      bool reviewExist = await dbContext.Reviews
        .AnyAsync(x => x.MovieId == movieId && x.UserId == userId);

      if (reviewExist) {
        return BadRequest("The user already has written a review of this movie.");
      }

      Review review = mapper.Map<Review>(reviewCreationDTO);
      review.MovieId = movieId;
      review.UserId = userId;

      dbContext.Add(review);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
