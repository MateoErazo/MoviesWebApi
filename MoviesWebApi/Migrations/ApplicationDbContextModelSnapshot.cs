﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoviesWebApi;

#nullable disable

namespace MoviesWebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MoviesWebApi.Entities.Actor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Picture")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Actors");

                    b.HasData(
                        new
                        {
                            Id = 7,
                            Birthdate = new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Jim Carrey"
                        },
                        new
                        {
                            Id = 8,
                            Birthdate = new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Robert Downey Jr."
                        },
                        new
                        {
                            Id = 9,
                            Birthdate = new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Chris Evans"
                        });
                });

            modelBuilder.Entity("MoviesWebApi.Entities.Gender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Genders");

                    b.HasData(
                        new
                        {
                            Id = 4,
                            Name = "Adventure"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Animation"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Suspense"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Romance"
                        });
                });

            modelBuilder.Entity("MoviesWebApi.Entities.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("InCinemas")
                        .HasColumnType("bit");

                    b.Property<string>("Poster")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Movies");

                    b.HasData(
                        new
                        {
                            Id = 4,
                            InCinemas = true,
                            ReleaseDate = new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Avengers: Endgame"
                        },
                        new
                        {
                            Id = 5,
                            InCinemas = false,
                            ReleaseDate = new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Avengers: Infinity Wars"
                        },
                        new
                        {
                            Id = 6,
                            InCinemas = false,
                            ReleaseDate = new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Sonic the Hedgehog"
                        },
                        new
                        {
                            Id = 7,
                            InCinemas = false,
                            ReleaseDate = new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Emma"
                        },
                        new
                        {
                            Id = 8,
                            InCinemas = false,
                            ReleaseDate = new DateTime(2020, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Wonder Woman 1984"
                        });
                });

            modelBuilder.Entity("MoviesWebApi.Entities.MovieActor", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("ActorId")
                        .HasColumnType("int");

                    b.Property<string>("CharacterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("MovieId", "ActorId");

                    b.HasIndex("ActorId");

                    b.ToTable("MoviesActors");

                    b.HasData(
                        new
                        {
                            MovieId = 4,
                            ActorId = 8,
                            CharacterName = "Tony Stark",
                            Order = 1
                        },
                        new
                        {
                            MovieId = 4,
                            ActorId = 9,
                            CharacterName = "Steve Rogers",
                            Order = 2
                        },
                        new
                        {
                            MovieId = 5,
                            ActorId = 8,
                            CharacterName = "Tony Stark",
                            Order = 1
                        },
                        new
                        {
                            MovieId = 5,
                            ActorId = 9,
                            CharacterName = "Steve Rogers",
                            Order = 2
                        },
                        new
                        {
                            MovieId = 6,
                            ActorId = 7,
                            CharacterName = "Dr. Ivo Robotnik",
                            Order = 1
                        });
                });

            modelBuilder.Entity("MoviesWebApi.Entities.MovieGender", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("GenderId")
                        .HasColumnType("int");

                    b.HasKey("MovieId", "GenderId");

                    b.HasIndex("GenderId");

                    b.ToTable("MoviesGenders");

                    b.HasData(
                        new
                        {
                            MovieId = 4,
                            GenderId = 6
                        },
                        new
                        {
                            MovieId = 4,
                            GenderId = 4
                        },
                        new
                        {
                            MovieId = 5,
                            GenderId = 6
                        },
                        new
                        {
                            MovieId = 5,
                            GenderId = 4
                        },
                        new
                        {
                            MovieId = 6,
                            GenderId = 4
                        },
                        new
                        {
                            MovieId = 7,
                            GenderId = 6
                        },
                        new
                        {
                            MovieId = 7,
                            GenderId = 7
                        },
                        new
                        {
                            MovieId = 8,
                            GenderId = 6
                        },
                        new
                        {
                            MovieId = 8,
                            GenderId = 4
                        });
                });

            modelBuilder.Entity("MoviesWebApi.Entities.MovieActor", b =>
                {
                    b.HasOne("MoviesWebApi.Entities.Actor", "Actor")
                        .WithMany("ActorMovies")
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoviesWebApi.Entities.Movie", "Movie")
                        .WithMany("MovieActors")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actor");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MoviesWebApi.Entities.MovieGender", b =>
                {
                    b.HasOne("MoviesWebApi.Entities.Gender", "Gender")
                        .WithMany("GenderMovies")
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoviesWebApi.Entities.Movie", "Movie")
                        .WithMany("MovieGenders")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Gender");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MoviesWebApi.Entities.Actor", b =>
                {
                    b.Navigation("ActorMovies");
                });

            modelBuilder.Entity("MoviesWebApi.Entities.Gender", b =>
                {
                    b.Navigation("GenderMovies");
                });

            modelBuilder.Entity("MoviesWebApi.Entities.Movie", b =>
                {
                    b.Navigation("MovieActors");

                    b.Navigation("MovieGenders");
                });
#pragma warning restore 612, 618
        }
    }
}
