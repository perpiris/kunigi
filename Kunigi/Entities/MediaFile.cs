using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class MediaFile
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Path { get; set; }
    
    public int ParentId { get; set; }
    
    public virtual ICollection<TeamMedia> TeamMediaFiles { get; set; }
    
    public virtual ICollection<ParentGameMedia> ParentGameMediaFiles { get; set; }
    
    public virtual ICollection<PuzzleMedia> PuzzleMediaFiles { get; set; }
}