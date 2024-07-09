using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Year
{
    public int YearId { get; set; }

    public short Value { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ImageUrl { get; set; }
    
    public int HostId { get; set; }
    public virtual Team Host { get; set; }
    
    public int WinnerId { get; set; }
    public virtual Team Winner { get; set; }
}