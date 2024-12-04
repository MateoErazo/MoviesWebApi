using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Services;

namespace MoviesWebApi.Controllers
{
  [ApiController]
  [Route("api/actors")]
  public class ActorsController : ControllerBase
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IFileStorer fileStorer;
    private readonly string containerName = "actors";

    public ActorsController(ApplicationDbContext dbContext, IMapper mapper, IFileStorer fileStorer)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.fileStorer = fileStorer;
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

      if (actorCreationDTO.Picture != null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          await actorCreationDTO.Picture.CopyToAsync(ms);
          byte[] content = ms.ToArray();
          var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
          actor.Picture = await fileStorer.SaveFileAsync(content:content,extension:extension,
            container:containerName,contentType:actorCreationDTO.Picture.ContentType);
        }
      }

      dbContext.Add(actor);
      await dbContext.SaveChangesAsync();
      ActorDTO actorDTO = mapper.Map<ActorDTO>(actor);
      return CreatedAtRoute("getActorById", new {id = actorDTO.Id}, actorDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromForm] ActorCreationDTO actorCreationDTO)
    {
      Actor actor = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);
      
      if (actor == null)
      {
        return NotFound($"Don't exist an actor with id {id}. Please check and try again.");
      }

      actor = mapper.Map(actorCreationDTO, actor);

      if (actorCreationDTO.Picture != null)
      {
        using (MemoryStream ms = new MemoryStream())
        {
          await actorCreationDTO.Picture.CopyToAsync(ms);
          byte[] content = ms.ToArray();
          var extension = Path.GetExtension(actorCreationDTO.Picture.FileName);
          actor.Picture = await fileStorer.EditFileAsync(content: content, extension: extension,
            container: containerName, path:actor.Picture, contentType: actorCreationDTO.Picture.ContentType);
        }
      }

      await dbContext.SaveChangesAsync();
      return NoContent(); 
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<ActorPatchDTO> patchDocument)
    {
      if(patchDocument == null)
      {
        return BadRequest();
      }

      Actor actorDB = await dbContext.Actors.FirstOrDefaultAsync(x => x.Id == id);

      if(actorDB == null)
      {
        return NotFound();
      }

      ActorPatchDTO actorPatchDTO = mapper.Map<ActorPatchDTO>(actorDB);

      patchDocument.ApplyTo(actorPatchDTO, ModelState);

      bool isValidModel = TryValidateModel(actorPatchDTO);

      if (!isValidModel)
      {
        return BadRequest(ModelState);
      }

      mapper.Map(actorPatchDTO, actorDB);

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
