using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
  public class MovieTheaterFilterNearDTO
  {
    [Range(-90, 90)]
    public double Latitude { get; set; }
    [Range(-180, 180)]
    public double Longitude { get; set; }
    private readonly double maximumDistance = 50;
    private readonly double minimumDistance = 0;
    private double distanceInKm = 2;
    public double DistanceInKm {
      get { return distanceInKm; }
      set { distanceInKm = (value > maximumDistance) ? maximumDistance :
                           (value < minimumDistance) ? minimumDistance : 
                           value; }
    }
  }
}
