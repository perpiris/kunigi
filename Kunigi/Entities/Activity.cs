using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Activity
{
    public int Id { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }

    public string Description { get; set; }
}