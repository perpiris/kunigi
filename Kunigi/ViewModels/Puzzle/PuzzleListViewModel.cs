namespace Kunigi.ViewModels.Puzzle;

public class PuzzleListViewModel
{
    public int GameId { get; set; }
    
    public int ParentGameYear { get; set; }
    
    public string GameType { get; set; }
    
    public List<PuzzleDetailsViewModel> Puzzles { get; set; }
    
    public PuzzleCreateViewModel CreatePuzzleViewModel { get; set; }
}