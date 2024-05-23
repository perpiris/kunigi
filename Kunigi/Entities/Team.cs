using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Team
{
    public int Id { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Name { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Slug { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Website { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Facebook { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Youtube { get; set; }
    
    [Column(TypeName = "varchar(150)")]
    public string Instagram { get; set; }
    
    public string ImageUrl { get; set; }
    
    public virtual ICollection<Game> HostedGames { get; set; }
    
    public virtual ICollection<Game> WonGames { get; set; }
}