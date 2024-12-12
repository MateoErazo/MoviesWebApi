using System.Runtime.InteropServices.Marshalling;

namespace MoviesWebApi.DTOs
{
  public class MoviesFilterDTO
  {
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public PaginationDTO Pagination {
      get { return new PaginationDTO() { Page = Page, PageSize = PageSize}; }
    }
    public string Title { get; set; }
    public int GenderId { get; set; }
    public bool InCinemas { get; set; }
    public bool ComingSoon { get; set; }

  }
}
