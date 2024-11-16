using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/actors")]
  public class ActorsController : ControllerBase
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public ActorsController(ApplicationDbContext dbContext, IMapper mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    [HttpGet(Name = "getAllActors")]
    public async Task<ActionResult<List<ActorDTO>>> GetAll()
    {
      List<Actor> actors = await dbContext.Actors.ToListAsync();
      return mapper.Map<List<ActorDTO>>(actors);
    }

    [HttpGet("{id:int}", Name = "getActorById")]
    public async Task<ActionResult<ActorDTO>> GetById(int id)
    {
      Actor actor = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);

      if (actor == null) {
        return NotFound($"Don't exist an actor with id {id}. Please check and try again.");
      }

      return mapper.Map<ActorDTO>(actor);
    }

    [HttpPost(Name = "createActor")]
    public async Task<ActionResult> Create([FromForm] ActorCreationDTO actorCreationDTO)
    {
      Actor actor = mapper.Map<Actor>(actorCreationDTO);
      dbContext.Add(actor);
      await dbContext.SaveChangesAsync();
      ActorDTO actorDTO = mapper.Map<ActorDTO>(actor);
      return CreatedAtRoute("getActorById", new {id = actorDTO.Id}, actorDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromForm] ActorCreationDTO actorCreationDTO)
    {
      bool actorExist = await dbContext.Actors.AnyAsync(x => x.Id == id);
      
      if (!actorExist)
      {
        return NotFound($"Don't exist an actor with id {id}. Please check and try again.");
      }

      Actor actor = mapper.Map<Actor>(actorCreationDTO);
      actor.Id = id;
      dbContext.Entry(actor).State = EntityState.Modified;
      await dbContext.SaveChangesAsync();
      return NoContent(); 
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
      bool actorExist = await dbContext.Actors.AnyAsync(x => x.Id == id);

      if (!actorExist)
      {
        return NotFound($"Don't exist an actor with id {id}. Please check and try again.");
      }

      dbContext.Remove(new Actor { Id = id});
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

  }
}
