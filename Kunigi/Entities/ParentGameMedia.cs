using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class ParentGameMedia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ParentGameMediaId { get; set; }
    
    public Guid ParentGameId { get; set; }
    
    public Guid MediaFileId { get; set; }
    
    public virtual ParentGame ParentGame { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}