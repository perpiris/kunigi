using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Puzzle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid PuzzleId { get; set; }
    
    public Guid GameId { get; set; }
    
    public short Order { get; set; }

    public short? Group { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    public virtual Game Game { get; set; }
    
    public virtual ICollection<PuzzleMedia> MediaFiles { get; set; }
}