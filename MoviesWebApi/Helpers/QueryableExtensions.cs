using MoviesWebApi.DTOs;

namespace MoviesWebApi.Helpers
{
  public static class QueryableExtensions
  {
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, 
      PaginationDTO paginationDTO)
    {
      return queryable.Skip((paginationDTO.Page - 1) * paginationDTO.PageSize)
        .Take(paginationDTO.PageSize);
    }
  }
}
