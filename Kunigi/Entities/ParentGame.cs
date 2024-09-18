using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class ParentGame
{
    public int ParentGameId { get; set; }

    public short Year { get; set; }

    public short Order { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string MainTitle { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string SubTitle { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ProfileImagePath { get; set; }
    
    public int HostId { get; set; }
    
    public int WinnerId { get; set; }
    
    [ForeignKey("HostId")]
    public virtual Team Host { get; set; }
    
    [ForeignKey("WinnerId")]
    public virtual Team Winner { get; set; }
    
    public virtual ICollection<Game> Games { get; set; }
    
    public virtual ICollection<ParentGameMedia> MediaFiles { get; set; }
}