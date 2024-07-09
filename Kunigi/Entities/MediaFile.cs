using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class MediaFile
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Path { get; set; }
    
    public int ParentId { get; set; }
}