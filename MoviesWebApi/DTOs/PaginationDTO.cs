namespace MoviesWebApi.DTOs
{
  public class PaginationDTO
  {
    private readonly int maximumRegistersByPage = 50;
    private int pageSize = 10;
    private int page = 1;
    private readonly int minimumPage = 1;
    public int Page {
      get => page;
      set
      {
        page = (value < minimumPage) ? minimumPage : value;
      }
    }
    public int PageSize {
      get => pageSize;
      set
      {
        pageSize = (value > maximumRegistersByPage) ? maximumRegistersByPage : value;
      }
    }
  }
}
