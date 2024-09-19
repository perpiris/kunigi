using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid GameId { get; set; }
    
    [MaxLength(150)]
    public string Title { get; set; }
    
    public string Description { get; set; }

    public Guid GameTypeId { get; set; }
    
    public Guid ParentGameId { get; set; }
    
    public virtual GameType GameType { get; set; }
    
    public virtual ParentGame ParentGame { get; set; }
    
    public virtual ICollection<Puzzle> PuzzleList { get; set; }
}