using Kunigi.Enums;

namespace Kunigi.Entities;

public class Puzzle
{
    public int PuzzleId { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }

    public int GameId { get; set; }
    
    public PuzzleType Type { get; set; } = PuzzleType.Main;

    public int Order { get; set; }
    
    public virtual Game Game { get; set; }
    
    public virtual ICollection<PuzzleMedia> MediaFiles { get; set; }
}