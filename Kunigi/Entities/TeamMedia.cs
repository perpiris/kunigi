using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class TeamMedia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid TeamMediaId { get; set; }
    
    public Guid TeamId { get; set; }
    
    public Guid MediaFileId { get; set; }
    
    public virtual Team Team { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}