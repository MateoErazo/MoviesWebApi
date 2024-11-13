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

    [HttpGet(Name = "getAllGenders")]
    public async Task<ActionResult<List<GenderDTO>>> GetAll()
    {
      List<Gender> genders = await dbContext.Genders.ToListAsync();
      return mapper.Map<List<GenderDTO>>(genders);
    }

    [HttpGet("{id:int}",Name = "getGenderById")]
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

      return CreatedAtRoute("getGenderById", new { id = genderDTO.Id }, genderDTO);
    }

    [HttpPut("{id:int}",Name ="updateGenderById")]
    public async Task<ActionResult> Update(int id, GenderCreationDTO genderCreationDTO)
    {
      bool genderExist = await dbContext.Genders.AnyAsync(x => x.Id == id);

      if (!genderExist) {
        return NotFound($"Don't exist a gender with id {id}.");
      }

      Gender gender = mapper.Map<Gender>(genderCreationDTO);
      gender.Id = id;
      dbContext.Update(gender);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{id:int}",Name = "deleteGenderById")]
    public async Task<ActionResult> Delete(int id)
    {
      bool genderExist = await dbContext.Genders.AnyAsync(x => x.Id == id);

      if (!genderExist)
      {
        return NotFound($"Don't exist a gender with id {id}.");
      }

      dbContext.Remove(new Gender { Id = id});
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
