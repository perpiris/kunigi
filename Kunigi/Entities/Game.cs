using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Game
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ImageUrl { get; set; }
    
    public int YearId { get; set; }
    public virtual Year Year { get; set; }
}