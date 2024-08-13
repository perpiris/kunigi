using Kunigi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<GameYear> GameYears { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<Puzzle> Puzzles { get; set; }
    public DbSet<TeamManager> TeamManagers { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<TeamMedia> TeamMediaFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<GameYear>()
            .HasOne(g => g.Host)
            .WithMany(t => t.HostedYears)
            .HasForeignKey(g => g.HostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GameYear>()
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
    }
}