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
    public DbSet<MediaFile> Files { get; set; }
    
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
    }
}