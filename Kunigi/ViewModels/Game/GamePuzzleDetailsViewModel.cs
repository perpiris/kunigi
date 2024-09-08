namespace Kunigi.ViewModels.Game;

public class GamePuzzleDetailsViewModel
{
    public GameDetailsViewModel GameDetails { get; set; }

    public List<PuzzleDetailsViewModel> PuzzleList { get; set; } = [];
}