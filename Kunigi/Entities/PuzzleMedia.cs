using Kunigi.Enums;

namespace Kunigi.Entities;

public class PuzzleMedia
{
    public int Id { get; set; }
    
    public int PuzzleId { get; set; }
    
    public int MediaFileId { get; set; }
    
    public PuzzleMediaType MediaType { get; set; }
    
    public virtual Puzzle Puzzle { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}