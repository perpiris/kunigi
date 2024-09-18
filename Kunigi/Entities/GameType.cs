using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class GameType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid GameTypeId { get; set; }

    [Column(TypeName = "varchar(255)")]
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
}