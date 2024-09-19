using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.Entities;

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
    
    [DisplayName("Επιλογές")]
    [Required(ErrorMessage = "Πρέπει να επιλέξετε τουλάχιστον έναν τύπο παιχνιδιού.")]
    [MinLength(1, ErrorMessage = "Πρέπει να επιλέξετε τουλάχιστον έναν τύπο παιχνιδιού.")]
    public List<Guid> SelectedGameTypeIds { get; set; } = [];
    
    public List<GameType> GameTypes { get; set; } = [];
}