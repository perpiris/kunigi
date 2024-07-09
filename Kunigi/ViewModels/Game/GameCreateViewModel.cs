using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Game;

public class GameCreateViewModel
{
    [DisplayName("Έτος διεξαγωγής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short Year { get; set; }
    
    [DisplayName("Διοργανωτής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int HostId { get; set; }
    
    [DisplayName("Νικητής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int WinnerId { get; set; }
    
    public SelectList HostSelectList { get; set; }
    
    public SelectList WinnerSelectList { get; set; }
    
    public List<string> Options { get; set; } = ["Χωρός", "Σάββατο", "Κυριακή", "Διαδικτυακό"];

    [DisplayName("Επιλογές")]
    public List<string> SelectedOptions { get; set; } = [];
}