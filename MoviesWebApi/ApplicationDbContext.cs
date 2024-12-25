using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Entities;
using System.Security.Claims;

namespace MoviesWebApi
{
  public class ApplicationDbContext : IdentityDbContext
  {
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<MovieActor>().HasKey( x => new {x.MovieId, x.ActorId});
      modelBuilder.Entity<MovieGender>().HasKey(x => new {x.MovieId, x.GenderId});
      modelBuilder.Entity<MovieTheaterMovie>().HasKey(x => new {x.MovieTheaterId, x.MovieId});
      SeedData(modelBuilder);
      base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
      string roleAdminId = "f2b68750-1e26-4ead-9999-3646e9bc978b";
      string userAdminId = "29dba010-2f36-4a69-86d9-9e5e795d507c";

      IdentityRole roleAdmin = new IdentityRole()
      {
        Id = roleAdminId,
        Name = "Admin",
        NormalizedName = "Admin"
      };

      var passwordHasher = new PasswordHasher<IdentityUser>();
      var userName = "rayoazul@gmail.com";

      var userAdmin = new IdentityUser()
      {
        Id = userAdminId,
        UserName = userName,
        NormalizedUserName = userName,
        Email = userName,
        NormalizedEmail = userName,
        PasswordHash = passwordHasher.HashPassword(null,"Aa!123456")
      };

      //modelBuilder.Entity<IdentityUser>().HasData(userAdmin);
      //modelBuilder.Entity<IdentityRole>().HasData(roleAdmin);
      //modelBuilder.Entity<IdentityUserClaim<string>>()
      //  .HasData(new IdentityUserClaim<string>()
      //  {
      //    Id = 1,
      //    ClaimType = ClaimTypes.Role,
      //    UserId = userAdminId,
      //    ClaimValue = "Admin"
      //  });



      var aventura = new Gender() { Id = 4, Name = "Adventure" };
      var animation = new Gender() { Id = 5, Name = "Animation" };
      var suspenso = new Gender() { Id = 6, Name = "Suspense" };
      var romance = new Gender() { Id = 7, Name = "Romance" };

      modelBuilder.Entity<Gender>()
          .HasData(new List<Gender>
          {
                    aventura, animation, suspenso, romance
          });

      var jimCarrey = new Actor() { Id = 7, Name = "Jim Carrey", Birthdate = new DateTime(1962, 01, 17) };
      var robertDowney = new Actor() { Id = 8, Name = "Robert Downey Jr.", Birthdate = new DateTime(1965, 4, 4) };
      var chrisEvans = new Actor() { Id = 9, Name = "Chris Evans", Birthdate = new DateTime(1981, 06, 13) };

      modelBuilder.Entity<Actor>()
          .HasData(new List<Actor>
          {
                    jimCarrey, robertDowney, chrisEvans
          });

      var endgame = new Movie()
      {
        Id = 4,
        Title = "Avengers: Endgame",
        InCinemas = true,
        ReleaseDate = new DateTime(2019, 04, 26)
      };

      var iw = new Movie()
      {
        Id = 5,
        Title = "Avengers: Infinity Wars",
        InCinemas = false,
        ReleaseDate = new DateTime(2019, 04, 26)
      };

      var sonic = new Movie()
      {
        Id = 6,
        Title = "Sonic the Hedgehog",
        InCinemas = false,
        ReleaseDate = new DateTime(2020, 02, 28)
      };
      var emma = new Movie()
      {
        Id = 7,
        Title = "Emma",
        InCinemas = false,
        ReleaseDate = new DateTime(2020, 02, 21)
      };
      var wonderwoman = new Movie()
      {
        Id = 8,
        Title = "Wonder Woman 1984",
        InCinemas = false,
        ReleaseDate = new DateTime(2020, 08, 14)
      };

      modelBuilder.Entity<Movie>()
          .HasData(new List<Movie>
          {
                    endgame, iw, sonic, emma, wonderwoman
          });

      modelBuilder.Entity<MovieGender>().HasData(
          new List<MovieGender>()
          {
                    new MovieGender(){MovieId = endgame.Id, GenderId = suspenso.Id},
                    new MovieGender(){MovieId = endgame.Id, GenderId = aventura.Id},
                    new MovieGender(){MovieId = iw.Id, GenderId = suspenso.Id},
                    new MovieGender(){MovieId = iw.Id, GenderId = aventura.Id},
                    new MovieGender(){MovieId = sonic.Id, GenderId = aventura.Id},
                    new MovieGender(){MovieId = emma.Id, GenderId = suspenso.Id},
                    new MovieGender(){MovieId = emma.Id, GenderId = romance.Id},
                    new MovieGender(){MovieId = wonderwoman.Id, GenderId = suspenso.Id},
                    new MovieGender(){MovieId = wonderwoman.Id, GenderId = aventura.Id},
          });

      modelBuilder.Entity<MovieActor>().HasData(
          new List<MovieActor>()
          {
                    new MovieActor(){MovieId = endgame.Id, ActorId = robertDowney.Id, CharacterName = "Tony Stark", Order = 1},
                    new MovieActor(){MovieId = endgame.Id, ActorId = chrisEvans.Id, CharacterName = "Steve Rogers", Order = 2},
                    new MovieActor(){MovieId = iw.Id, ActorId = robertDowney.Id, CharacterName = "Tony Stark", Order = 1},
                    new MovieActor(){MovieId = iw.Id, ActorId = chrisEvans.Id, CharacterName = "Steve Rogers", Order = 2},
                    new MovieActor(){MovieId = sonic.Id, ActorId = jimCarrey.Id, CharacterName = "Dr. Ivo Robotnik", Order = 1}
          });
    }

    public DbSet<Gender> Genders { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieActor> MoviesActors { get; set; }
    public DbSet<MovieGender> MoviesGenders { get; set; }
    public DbSet<MovieTheater> MovieTheaters { get; set; }
    public DbSet<MovieTheaterMovie> MovieTheatersMovies { get;set; }
  }
}
