using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Helpers;
using MoviesWebApi.Interfaces;

namespace MoviesWebApi.Controllers
{
  public class CustomBaseController: ControllerBase
  {
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public CustomBaseController(ApplicationDbContext dbContext, IMapper mapper)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
    }

    protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
    {
      List<TEntity> entities = await dbContext.Set<TEntity>().ToListAsync();
      List<TDTO> dtos = mapper.Map<List<TDTO>>(entities);
      return dtos;
    }

    protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO paginationDTO) where TEntity : class
    {
      var queryable = dbContext.Set<TEntity>().AsQueryable();
      return await Get<TEntity, TDTO>(paginationDTO, queryable);
    }

    protected async Task<List<TDTO>> Get<TEntity, TDTO>(
      PaginationDTO paginationDTO, IQueryable<TEntity> queryable) where TEntity : class
    {
      await HttpContext.InsertParametersPagination(queryable, paginationDTO.PageSize);

      List<TEntity> entities = await queryable
        .Paginate(paginationDTO)
        .ToListAsync();

      return mapper.Map<List<TDTO>>(entities);
    }

    protected async Task<ActionResult<TDTO>> GetById<TEntity, TDTO>(int id) where TEntity : class, IId
    {
      TEntity entity = await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

      if (entity == null) {
        return NotFound();
      }

      return mapper.Map<TDTO>(entity);
    }

    protected async Task<ActionResult> Create<TCreationDTO, TEntity, TResultDTO>
      (TCreationDTO creationDTO, string routeName) where TEntity : class, IId
    {
      TEntity entity = mapper.Map<TEntity>(creationDTO);
      dbContext.Add(entity);
      await dbContext.SaveChangesAsync();

      TResultDTO resultDTO = mapper.Map<TResultDTO>(entity);
      return CreatedAtRoute(routeName, new {id = entity.Id}, resultDTO);
    }

    protected async Task<ActionResult> Update<TCreationDTO, TEntity>(int id, TCreationDTO creationDTO) where TEntity : class, IId
    {
      bool entityExist = await dbContext.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id);

      if (!entityExist) {
        return NotFound();
      }

      TEntity entity = mapper.Map<TEntity>(creationDTO);
      entity.Id = id;
      dbContext.Update(entity);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
    {
      bool entityExist = await dbContext.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id);

      if (!entityExist)
      {
        return NotFound();
      }

      dbContext.Remove(new TEntity { Id = id});
      await dbContext.SaveChangesAsync();
      return NoContent();
    }

    protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument)
      where TEntity : class, IId
      where TDTO: class
    {
      if (patchDocument == null) {
        return BadRequest();  
      }

      TEntity entityDb = await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
      if (entityDb == null) { 
        return NotFound();
      }

      TDTO patchDTO = mapper.Map<TDTO>(entityDb);

      patchDocument.ApplyTo(patchDTO,ModelState);

      bool isValidModel = TryValidateModel(patchDTO);
      if (!isValidModel) { 
        return BadRequest(ModelState);
      }

      mapper.Map(patchDTO, entityDb);
      await dbContext.SaveChangesAsync();
      return NoContent();
    }
  }
}
