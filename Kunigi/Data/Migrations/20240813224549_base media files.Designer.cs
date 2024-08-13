﻿// <auto-generated />
using System;
using Kunigi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kunigi.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240813224549_base media files")]
    partial class basemediafiles
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("AppUserTeam", b =>
                {
                    b.Property<int>("ManagedTeamsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ManagersId")
                        .HasColumnType("TEXT");

                    b.HasKey("ManagedTeamsId", "ManagersId");

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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameYearId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("GameYearId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Kunigi.Entities.GameType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("Kunigi.Entities.GameYear", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("HostId")
                        .HasColumnType("INTEGER");

                    b.Property<short>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("TeamFolderUrl")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.Property<short>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("HostId");

                    b.HasIndex("WinnerId");

                    b.ToTable("GameYears");
                });

            modelBuilder.Entity("Kunigi.Entities.MediaFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameYearId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ParentType")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Path")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("PuzzleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameYearId");

                    b.HasIndex("PuzzleId");

                    b.HasIndex("ParentId", "ParentType");

                    b.ToTable("MediaFiles");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Answer")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Question")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Puzzles");
                });

            modelBuilder.Entity("Kunigi.Entities.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Facebook")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Instagram")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("TeamFolderUrl")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Website")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("Youtube")
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Kunigi.Entities.TeamManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AppUserId")
                        .HasColumnType("TEXT");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamManagers");
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
                        .HasForeignKey("ManagedTeamsId")
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

                    b.HasOne("Kunigi.Entities.GameYear", "GameYear")
                        .WithMany("Games")
                        .HasForeignKey("GameYearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameType");

                    b.Navigation("GameYear");
                });

            modelBuilder.Entity("Kunigi.Entities.GameYear", b =>
                {
                    b.HasOne("Kunigi.Entities.Team", "Host")
                        .WithMany("HostedYears")
                        .HasForeignKey("HostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kunigi.Entities.Team", "Winner")
                        .WithMany("WonYears")
                        .HasForeignKey("WinnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Host");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("Kunigi.Entities.MediaFile", b =>
                {
                    b.HasOne("Kunigi.Entities.GameYear", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("GameYearId");

                    b.HasOne("Kunigi.Entities.Game", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MediaFiles_Games");

                    b.HasOne("Kunigi.Entities.Team", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MediaFiles_Teams");

                    b.HasOne("Kunigi.Entities.Puzzle", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("PuzzleId");
                });

            modelBuilder.Entity("Kunigi.Entities.Puzzle", b =>
                {
                    b.HasOne("Kunigi.Entities.Game", "Game")
                        .WithMany("Puzzles")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
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
                    b.Navigation("MediaFiles");

                    b.Navigation("Puzzles");
                });

            modelBuilder.Entity("Kunigi.Entities.GameYear", b =>
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
                    b.Navigation("HostedYears");

                    b.Navigation("MediaFiles");

                    b.Navigation("WonYears");
                });
#pragma warning restore 612, 618
        }
    }
}
