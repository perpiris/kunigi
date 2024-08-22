namespace Kunigi.Entities;

public class ParentGameMedia
{
    public int Id { get; set; }
    
    public int ParentGameId { get; set; }
    
    public int MediaFileId { get; set; }
    
    public virtual ParentGame ParentGame { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}