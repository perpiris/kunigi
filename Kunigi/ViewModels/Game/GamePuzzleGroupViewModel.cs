namespace Kunigi.ViewModels.Game;

public class GamePuzzleGroupViewModel
{
    public string GroupName { get; set; }
    
    public short MinOrder { get; set; }
    
    public List<PuzzleDetailsViewModel> Puzzles { get; set; } = [];
}