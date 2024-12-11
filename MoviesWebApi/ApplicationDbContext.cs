using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Entities;

namespace MoviesWebApi
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<MovieActor>().HasKey( x => new {x.MovieId, x.ActorId});
      modelBuilder.Entity<MovieGender>().HasKey(x => new {x.MovieId, x.GenderId});
      base.OnModelCreating(modelBuilder);
    }

    public DbSet<Gender> Genders { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieActor> MoviesActors { get; set; }
    public DbSet<MovieGender> MoviesGenders { get; set; }
  }
}
