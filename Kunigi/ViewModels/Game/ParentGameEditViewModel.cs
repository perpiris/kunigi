using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class ParentGameEditViewModel
{
    [DisplayName("Τίτλος")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string SubTitle { get; set; }

    [DisplayName("Περιγραφή")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Description { get; set; }
    
    [DisplayName("Αφίσα")]
    public string ProfileImageUrl { get; set; }
    
    public short GameYear { get; set; }
}