namespace Kunigi.ViewModels.Puzzle;

public class GamePuzzlesViewModel
{
    public int Id { get; set; }

    public string Title { get; set; }
    
    public short Year { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
    
    public List<PuzzleDetailsViewModel> Puzzles { get; set; } = [];
}
