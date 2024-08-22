using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Puzzle;

public class PuzzleCreateViewModel
{
    [Required]
    public int GameId { get; set; }

    [DisplayName("Περιγραφή Γρίφου")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Question { get; set; }

    [DisplayName("Περιγραφή Απάντησης")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Answer { get; set; }

    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Type { get; set; }

    [DisplayName("Υλικό Γρίφου")]
    public List<IFormFile> QuestionMediaFiles { get; set; }

    [DisplayName("Υλικό Απάντησης")]
    public List<IFormFile> AnswerMediaFiles { get; set; }
    
    public List<SelectListItem> PuzzleTypes { get; set; }
}