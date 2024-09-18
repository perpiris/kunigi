using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class MediaFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid MediaFileId { get; set; }
    
    public string Path { get; set; }
    
    public virtual ICollection<TeamMedia> TeamMediaFiles { get; set; }
    
    public virtual ICollection<ParentGameMedia> ParentGameMediaFiles { get; set; }
    
    public virtual ICollection<PuzzleMedia> PuzzleMediaFiles { get; set; }
}