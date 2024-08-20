using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class GameType
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(255)")]
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
}