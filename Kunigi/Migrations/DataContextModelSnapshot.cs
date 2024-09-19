﻿// <auto-generated />
using System;
using Kunigi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kunigi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Kunigi.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Kunigi.Entities.Game", b =>
                {
                    b.Property<Guid>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("GameTypeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ParentGameId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.HasKey("GameId");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("ParentGameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Kunigi.Entities.GameType", b =>
                {
                    b.Property<Guid>("GameTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(255)");

                    b.HasKey("GameTypeId");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("Kunigi.Entities.MediaFile", b =>
                {
                    b.Property<Guid>("MediaFileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.HasKey("MediaFileId");

                    b.ToTable("MediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGame", b =>
                {
                    b.Property<Guid>("ParentGameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("HostId")
                        .HasColumnType("uuid");

                    b.Property<string>("MainTitle")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<short>("Order")
                        .HasColumnType("smallint");

                    b.Property<string>("ProfileImagePath")
                        .HasColumnType("text");

                    b.Property<string>("Slug")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("SubTitle")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<Guid>("WinnerId")
                        .HasColumnType("uuid");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("ParentGameId");

                    b.HasIndex("HostId");

                    b.HasIndex("WinnerId");

                    b.ToTable("ParentGames");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGameMedia", b =>
                {
                    b.Property<Guid>("ParentGameMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediaFileId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ParentGameId")
                        .HasColumnType("uuid");

                    b.HasKey("ParentGameMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("ParentGameId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("ParentGameMediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.Property<Guid>("PuzzleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answer")
                        .HasColumnType("text");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uuid");

                    b.Property<short?>("Group")
                        .HasColumnType("smallint");

                    b.Property<short>("Order")
                        .HasColumnType("smallint");

                    b.Property<string>("Question")
                        .HasColumnType("text");

                    b.HasKey("PuzzleId");

                    b.HasIndex("GameId");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("Kunigi.Entities.PuzzleMedia", b =>
                {
                    b.Property<Guid>("PuzzleMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediaFileId")
                        .HasColumnType("uuid");

                    b.Property<string>("MediaType")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<Guid>("PuzzleId")
                        .HasColumnType("uuid");

                    b.HasKey("PuzzleMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("PuzzleId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("PuzzleMediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Team", b =>
                {
                    b.Property<Guid>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<short?>("CreatedYear")
                        .HasColumnType("smallint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Facebook")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("Instagram")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("ProfileImagePath")
                        .HasColumnType("text");

                    b.Property<string>("Slug")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("Website")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<string>("Youtube")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamManager", b =>
                {
                    b.Property<Guid>("TeamManagerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AppUserId")
                        .HasColumnType("text");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("TeamManagerId");

                    b.HasIndex("AppUserId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamManagers");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamMedia", b =>
                {
                    b.Property<Guid>("TeamMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MediaFileId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("TeamMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("TeamId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("TeamMediaFiles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Kunigi.Entities.Game", b =>
                {
                    b.HasOne("Kunigi.Entities.GameType", "GameType")
                        .WithMany()
                        .HasForeignKey("GameTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.ParentGame", "ParentGame")
                        .WithMany("Games")
                        .HasForeignKey("ParentGameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameType");

                    b.Navigation("ParentGame");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGame", b =>
                {
                    b.HasOne("Kunigi.Entities.Team", "Host")
                        .WithMany("HostedGames")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.Team", "Winner")
                        .WithMany("WonGames")
                        .HasForeignKey("WinnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Host");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGameMedia", b =>
                {
                    b.HasOne("Kunigi.Entities.MediaFile", "MediaFile")
                        .WithMany("ParentGameMediaFiles")
                        .HasForeignKey("MediaFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.ParentGame", "ParentGame")
                        .WithMany("MediaFiles")
                        .HasForeignKey("ParentGameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MediaFile");

                    b.Navigation("ParentGame");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.HasOne("Kunigi.Entities.Game", "Game")
                        .WithMany("PuzzleList")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Kunigi.Entities.PuzzleMedia", b =>
                {
                    b.HasOne("Kunigi.Entities.MediaFile", "MediaFile")
                        .WithMany("PuzzleMediaFiles")
                        .HasForeignKey("MediaFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.Puzzle", "Puzzle")
                        .WithMany("MediaFiles")
                        .HasForeignKey("PuzzleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MediaFile");

                    b.Navigation("Puzzle");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamManager", b =>
                {
                    b.HasOne("Kunigi.Entities.AppUser", "User")
                        .WithMany("ManagedTeams")
                        .HasForeignKey("AppUserId");

                    b.HasOne("Kunigi.Entities.Team", "Team")
                        .WithMany("Managers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamMedia", b =>
                {
                    b.HasOne("Kunigi.Entities.MediaFile", "MediaFile")
                        .WithMany("TeamMediaFiles")
                        .HasForeignKey("MediaFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.Team", "Team")
                        .WithMany("MediaFiles")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MediaFile");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Kunigi.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Kunigi.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Kunigi.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kunigi.Entities.AppUser", b =>
                {
                    b.Navigation("ManagedTeams");
                });

            modelBuilder.Entity("Kunigi.Entities.Game", b =>
                {
                    b.Navigation("PuzzleList");
                });

            modelBuilder.Entity("Kunigi.Entities.MediaFile", b =>
                {
                    b.Navigation("ParentGameMediaFiles");

                    b.Navigation("PuzzleMediaFiles");

                    b.Navigation("TeamMediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGame", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("MediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.Navigation("MediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Team", b =>
                {
                    b.Navigation("HostedGames");

                    b.Navigation("Managers");

                    b.Navigation("MediaFiles");

                    b.Navigation("WonGames");
                });
#pragma warning restore 612, 618
        }
    }
}
