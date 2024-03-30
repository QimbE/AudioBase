﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240330062136_addFavorites")]
    partial class addFavorites
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Artists.Artist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhotoLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Domain.Favorites.Favorite", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.HasIndex("UserId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Domain.Junctions.CoAuthor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CoAuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CoAuthorId");

                    b.HasIndex("TrackId");

                    b.ToTable("CoAuthors");
                });

            modelBuilder.Entity("Domain.Junctions.LabelRelease", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("LabelId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ReleaseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("LabelId");

                    b.HasIndex("ReleaseId");

                    b.ToTable("LabelReleases");
                });

            modelBuilder.Entity("Domain.Labels.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("PhotoLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("Domain.MusicReleases.Release", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("CoverLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("date");

                    b.Property<int>("ReleaseTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ReleaseTypeId");

                    b.ToTable("Releases");
                });

            modelBuilder.Entity("Domain.MusicReleases.ReleaseType", b =>
                {
                    b.Property<int>("Value")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Value"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Value");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ReleaseTypes");

                    b.HasData(
                        new
                        {
                            Value = 2,
                            Name = "Album"
                        },
                        new
                        {
                            Value = 3,
                            Name = "Bootleg"
                        },
                        new
                        {
                            Value = 8,
                            Name = "Compilation"
                        },
                        new
                        {
                            Value = 4,
                            Name = "EP"
                        },
                        new
                        {
                            Value = 5,
                            Name = "LP"
                        },
                        new
                        {
                            Value = 6,
                            Name = "Live"
                        },
                        new
                        {
                            Value = 7,
                            Name = "Mixtape"
                        },
                        new
                        {
                            Value = 1,
                            Name = "Single"
                        });
                });

            modelBuilder.Entity("Domain.Tracks.Genre", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Domain.Tracks.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AudioLink")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<Guid>("GenreId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("ReleaseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GenreId");

                    b.HasIndex("ReleaseId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Domain.Users.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Value");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Domain.Users.Role", b =>
                {
                    b.Property<int>("Value")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Value"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Value");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Value = 3,
                            Name = "Admin"
                        },
                        new
                        {
                            Value = 2,
                            Name = "CatalogAdmin"
                        },
                        new
                        {
                            Value = 1,
                            Name = "DefaultUser"
                        });
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Infrastructure.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("OutboxMessages", (string)null);
                });

            modelBuilder.Entity("Infrastructure.Outbox.OutboxMessageConsumer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("ConsumerName")
                        .HasColumnType("text");

                    b.HasKey("Id", "ConsumerName");

                    b.ToTable("OutboxMessageConsumers");
                });

            modelBuilder.Entity("Domain.Favorites.Favorite", b =>
                {
                    b.HasOne("Domain.Tracks.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Users.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Junctions.CoAuthor", b =>
                {
                    b.HasOne("Domain.Artists.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("CoAuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Tracks.Track", "Track")
                        .WithMany("CoAuthors")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("Domain.Junctions.LabelRelease", b =>
                {
                    b.HasOne("Domain.Labels.Label", "Label")
                        .WithMany("LabelReleases")
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.MusicReleases.Release", "Release")
                        .WithMany("LabelReleases")
                        .HasForeignKey("ReleaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Label");

                    b.Navigation("Release");
                });

            modelBuilder.Entity("Domain.MusicReleases.Release", b =>
                {
                    b.HasOne("Domain.Artists.Artist", "Author")
                        .WithMany("Releases")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.MusicReleases.ReleaseType", "ReleaseType")
                        .WithMany()
                        .HasForeignKey("ReleaseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("ReleaseType");
                });

            modelBuilder.Entity("Domain.Tracks.Track", b =>
                {
                    b.HasOne("Domain.Artists.Artist", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Tracks.Genre", "Genre")
                        .WithMany("Tracks")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.MusicReleases.Release", "Release")
                        .WithMany("Tracks")
                        .HasForeignKey("ReleaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Genre");

                    b.Navigation("Release");
                });

            modelBuilder.Entity("Domain.Users.RefreshToken", b =>
                {
                    b.HasOne("Domain.Users.User", "Owner")
                        .WithOne("RefreshToken")
                        .HasForeignKey("Domain.Users.RefreshToken", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.HasOne("Domain.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Artists.Artist", b =>
                {
                    b.Navigation("Releases");
                });

            modelBuilder.Entity("Domain.Labels.Label", b =>
                {
                    b.Navigation("LabelReleases");
                });

            modelBuilder.Entity("Domain.MusicReleases.Release", b =>
                {
                    b.Navigation("LabelReleases");

                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("Domain.Tracks.Genre", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("Domain.Tracks.Track", b =>
                {
                    b.Navigation("CoAuthors");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("RefreshToken")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
