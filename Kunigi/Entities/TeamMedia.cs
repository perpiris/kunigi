namespace Kunigi.Entities;

public class TeamMedia
{
    public int TeamMediaId { get; set; }
    
    public int TeamId { get; set; }
    
    public int MediaFileId { get; set; }
    
    public virtual Team Team { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}