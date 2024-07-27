namespace Kunigi.Entities;

public class TeamManager
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int AppUserId { get; set; }
    
    public virtual Team Team { get; set; }
    
    public virtual AppUser User { get; set; }
}