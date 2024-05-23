using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Game
{
    public int Id { get; set; }
    
    public short Year { get; set; }

    public short Order { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ImageUrl { get; set; }
    
    public int HostId { get; set; }
    public virtual Team Host { get; set; }
    
    public int WinnerId { get; set; }
    public virtual Team Winner { get; set; }
}