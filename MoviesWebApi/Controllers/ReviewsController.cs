using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Filters;
using System.Security.Claims;
namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/movies/{movieId:int}/reviews")]
  [ServiceFilter(typeof(MovieExistAttribute))]
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

    [HttpPut("{reviewId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Update(int movieId, int reviewId, ReviewCreationDTO reviewCreationDTO)
    {
      Review review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
      if (review == null) {
        return NotFound();
      }

      string userId = HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

      if(review.UserId != userId)
      {
        return BadRequest("This review was created by other user. You don't have permission to update It.");
      }

      review = mapper.Map(reviewCreationDTO, review);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{reviewId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Delete(int reviewId)
    {
      Review review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
      if (review == null) { return NotFound(); }

      string userId = HttpContext.User.Claims.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier).Value;
      if (review.UserId != userId) { return BadRequest("This review was created by other user. You don't have permission to delete It."); }
      dbContext.Remove(review);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }


  }
}
