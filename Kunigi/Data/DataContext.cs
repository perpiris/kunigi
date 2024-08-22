using Kunigi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<ParentGame> ParentGames { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<Puzzle> Puzzles { get; set; }
    public DbSet<TeamManager> TeamManagers { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<TeamMedia> TeamMediaFiles { get; set; }
    public DbSet<ParentGameMedia> GameYearMediaFiles { get; set; }
    public DbSet<PuzzleMedia> PuzzleMediaFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ParentGame>()
            .HasOne(g => g.Host)
            .WithMany(t => t.HostedYears)
            .HasForeignKey(g => g.HostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ParentGame>()
            .HasOne(g => g.Winner)
            .WithMany(t => t.WonYears)
            .HasForeignKey(g => g.WinnerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TeamMedia>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Team)
                .WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.MediaFile)
                .WithMany(p => p.TeamMediaFiles)
                .HasForeignKey(d => d.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.TeamId, e.MediaFileId }).IsUnique();
        });
        
        modelBuilder.Entity<ParentGameMedia>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.ParentGame)
                .WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.ParentGameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.MediaFile)
                .WithMany(p => p.ParentGameMediaFiles)
                .HasForeignKey(d => d.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { GameYearId = e.ParentGameId, e.MediaFileId }).IsUnique();
        });
        
        modelBuilder.Entity<PuzzleMedia>(entity =>
        {
            entity.HasKey(e => e.Id);

            modelBuilder.Entity<Puzzle>()
                .HasMany(p => p.MediaFiles)
                .WithOne(pm => pm.Puzzle)
                .HasForeignKey(pm => pm.PuzzleId);

            entity.HasOne(d => d.MediaFile)
                .WithMany(p => p.PuzzleMediaFiles)
                .HasForeignKey(d => d.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PuzzleId, e.MediaFileId }).IsUnique();
        });
    }
}