using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class GamePuzzleCreateViewModel
{
    [Required]
    public int GameId { get; set; }

    [DisplayName("Περιγραφή Γρίφου")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Question { get; set; }

    [DisplayName("Περιγραφή Απάντησης")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Answer { get; set; }

    [DisplayName("Υλικό Γρίφου")]
    public List<IFormFile> QuestionMediaFiles { get; set; }

    [DisplayName("Υλικό Απάντησης")]
    public List<IFormFile> AnswerMediaFiles { get; set; }
}