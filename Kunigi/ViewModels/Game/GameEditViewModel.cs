using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class GameEditViewModel
{
    public int Id { get; set; }
    
    [DisplayName("Τίτλος")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Title { get; set; }
    
    [DisplayName("Σύντομη περιγραφή")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Description { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ImageUrl { get; set; }
}