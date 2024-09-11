using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Team;

public class TeamEditViewModel
{
    public string TeamSlug { get; set; }
    
    [DisplayName("Όνομα Ομάδας")]
    public string Name { get; set; }
    
    [DisplayName("Έτος Δημιουργίας")]
    [Range(1900, 2100, ErrorMessage = "Παρακαλώ εισάγετε έγκυρο έτος.")]
    public short? CreatedYear { get; set; }

    [DisplayName("Ενεργή")]
    public bool IsActive { get; set; }
    
    [DisplayName("Περιγραφή")]
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ProfileImageUrl { get; set; }
}