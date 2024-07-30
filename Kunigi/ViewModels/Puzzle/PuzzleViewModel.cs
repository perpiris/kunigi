using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Puzzle;

public class PuzzleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Question is required")]
    public string Question { get; set; }

    [Required(ErrorMessage = "Answer is required")]
    public string Answer { get; set; }

    public int GameId { get; set; }
}