using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/genders")]
  public class GendersController:ControllerBase
  {
    private readonly IMapper mapper;
    private readonly ApplicationDbContext dbContext;

    public GendersController(IMapper mapper, ApplicationDbContext dbContext)
    {
      this.mapper = mapper;
      this.dbContext = dbContext;
    }

    [HttpGet(Name = "GetAllGenders")]
    public async Task<ActionResult<List<GenderDTO>>> GetAll()
    {
      List<Gender> genders = await dbContext.Genders.ToListAsync();
      return mapper.Map<List<GenderDTO>>(genders);
    }

    [HttpGet("{id:int}",Name = "GetGenderById")]
    public async Task<ActionResult<GenderDTO>> GetById(int id)
    {
      Gender gender = await dbContext.Genders.FirstOrDefaultAsync(x => x.Id == id);

      if(gender == null)
      {
        return NotFound($"Don't exist a gender with id {id}.");
      }

      return mapper.Map<GenderDTO>(gender);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] GenderCreationDTO genderCreationDTO)
    {
      Gender gender = mapper.Map<Gender>(genderCreationDTO);
      dbContext.Add(gender);
      await dbContext.SaveChangesAsync();

      GenderDTO genderDTO = mapper.Map<GenderDTO>(gender);

      return CreatedAtRoute("GetGenderById", new { id = genderDTO.Id }, genderDTO);
    }
  }
}
