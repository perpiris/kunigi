namespace Kunigi.ViewModels.Puzzle;

public class GamePuzzlesViewModel
{
    public int GameId { get; set; }
    public string GameType { get; set; }
    public List<PuzzleDetailsViewModel> Puzzles { get; set; } = new();
}
