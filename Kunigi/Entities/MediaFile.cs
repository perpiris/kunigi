using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class MediaFile
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Path { get; set; }
    
    public int ParentId { get; set; }
    
    [Column(TypeName = "varchar(50)")]
    public string ParentType { get; set; }
    
    public virtual ICollection<TeamMedia> TeamMediaFiles { get; set; }
    
    public virtual ICollection<ParentGameMedia> GameYearMediaFiles { get; set; }
    
    public virtual ICollection<PuzzleMedia> PuzzleMediaFiles { get; set; }
}