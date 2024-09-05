namespace Kunigi.Entities;

public class TeamManager
{
    public int TeamManagerId { get; set; }

    public int TeamId { get; set; }

    public string AppUserId { get; set; }
    
    public virtual Team Team { get; set; }
    
    public virtual AppUser User { get; set; }
}