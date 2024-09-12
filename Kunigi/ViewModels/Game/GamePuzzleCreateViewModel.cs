using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class GamePuzzleCreateViewModel
{
    [Required]
    public int GameId { get; set; }

    [DisplayName("Περιγραφή Γρίφου")]
    public string Question { get; set; }

    [DisplayName("Περιγραφή Απάντησης")]
    public string Answer { get; set; }
    
    [DisplayName("Πακέτο Γρίφου")]
    public short? Group { get; set; }

    [DisplayName("Υλικό Γρίφου")]
    public List<IFormFile> QuestionMediaFiles { get; set; }

    [DisplayName("Υλικό Απάντησης")]
    public List<IFormFile> AnswerMediaFiles { get; set; }
    
    public string GameTypeSlug { get; set; }
    
    public short GameYear { get; set; }
}