using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/genders")]
  public class GendersController:CustomBaseController
  {
    public GendersController(IMapper mapper, ApplicationDbContext dbContext)
      :base(dbContext: dbContext, mapper: mapper){}

    [HttpGet(Name = "getAllGenders")]
    public async Task<ActionResult<List<GenderDTO>>> GetAll()
    {
      return await Get<Gender, GenderDTO>();
    }

    [HttpGet("{id:int}",Name = "getGenderById")]
    public async Task<ActionResult<GenderDTO>> GetById(int id)
    {
      return await GetById<Gender, GenderDTO>(id);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] GenderCreationDTO genderCreationDTO)
    {
      return await Create<GenderCreationDTO, Gender, GenderDTO>(genderCreationDTO, "getGenderById");
    }

    [HttpPut("{id:int}",Name ="updateGenderById")]
    public async Task<ActionResult> Update(int id, GenderCreationDTO genderCreationDTO)
    {
      return await Update<GenderCreationDTO, Gender>(id, genderCreationDTO);
    }

    [HttpDelete("{id:int}",Name = "deleteGenderById")]
    public async Task<ActionResult> Delete(int id)
    {
      return await Delete<Gender>(id);
    }
  }
}
