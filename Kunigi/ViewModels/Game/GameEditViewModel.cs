using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class GameEditViewModel
{
    public Guid GameId { get; set; }
    
    [DisplayName("Τίτλος")]
    [StringLength(150)]
    public string Title { get; set; }

    [DisplayName("Περιγραφή")]
    public string Description { get; set; }
}