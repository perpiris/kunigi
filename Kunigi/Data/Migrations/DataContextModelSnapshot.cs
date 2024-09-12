﻿// <auto-generated />
using System;
using Kunigi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kunigi.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("AppUserTeam", b =>
                {
                    b.Property<int>("ManagedTeamsTeamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ManagersId")
                        .HasColumnType("TEXT");

                    b.HasKey("ManagedTeamsTeamId", "ManagersId");

                    b.HasIndex("ManagersId");

                    b.ToTable("AppUserTeam");
                });

            modelBuilder.Entity("Kunigi.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

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
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParentGameId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GameId");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("ParentGameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Kunigi.Entities.GameType", b =>
                {
                    b.Property<int>("GameTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(255)");

                    b.HasKey("GameTypeId");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("Kunigi.Entities.MediaFile", b =>
                {
                    b.Property<int>("MediaFileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("varchar(255)");

                    b.HasKey("MediaFileId");

                    b.ToTable("MediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGame", b =>
                {
                    b.Property<int>("ParentGameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("HostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MainTitle")
                        .HasColumnType("varchar(255)");

                    b.Property<short>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ParentGameFolderPath")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ParentGameProfileImagePath")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SubTitle")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.Property<short>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("ParentGameId");

                    b.HasIndex("HostId");

                    b.HasIndex("WinnerId");

                    b.ToTable("ParentGames");
                });

            modelBuilder.Entity("Kunigi.Entities.ParentGameMedia", b =>
                {
                    b.Property<int>("ParentGameMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediaFileId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParentGameId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ParentGameMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("ParentGameId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("ParentGameMediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.Property<int>("PuzzleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Answer")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<short?>("Group")
                        .HasColumnType("INTEGER");

                    b.Property<short>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Question")
                        .HasColumnType("TEXT");

                    b.HasKey("PuzzleId");

                    b.HasIndex("GameId");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("Kunigi.Entities.PuzzleMedia", b =>
                {
                    b.Property<int>("PuzzleMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediaFileId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediaType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PuzzleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PuzzleMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("PuzzleId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("PuzzleMediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<short?>("CreatedYear")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Facebook")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Instagram")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TeamFolderPath")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TeamProfileImagePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Website")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Youtube")
                        .HasColumnType("varchar(255)");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamManager", b =>
                {
                    b.Property<int>("TeamManagerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AppUserId")
                        .HasColumnType("TEXT");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamManagerId");

                    b.HasIndex("AppUserId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamManagers");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamMedia", b =>
                {
                    b.Property<int>("TeamMediaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MediaFileId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamMediaId");

                    b.HasIndex("MediaFileId");

                    b.HasIndex("TeamId", "MediaFileId")
                        .IsUnique();

                    b.ToTable("TeamMediaFiles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

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
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AppUserTeam", b =>
                {
                    b.HasOne("Kunigi.Entities.Team", null)
                        .WithMany()
                        .HasForeignKey("ManagedTeamsTeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("ManagersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                        .WithMany()
                        .HasForeignKey("AppUserId");

                    b.HasOne("Kunigi.Entities.Team", "Team")
                        .WithMany()
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

                    b.Navigation("MediaFiles");

                    b.Navigation("WonGames");
                });
#pragma warning restore 612, 618
        }
    }
}
