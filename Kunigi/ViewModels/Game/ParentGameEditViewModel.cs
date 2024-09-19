using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class ParentGameEditViewModel
{
    public Guid ParentGameId { get; set; }
    
    [DisplayName("Τίτλος")]
    [StringLength(150)]
    public string SubTitle { get; set; }

    [DisplayName("Περιγραφή")]
    public string Description { get; set; }
    
    [DisplayName("Αφίσα")]
    public string ProfileImageUrl { get; set; }
}