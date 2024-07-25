using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Team;

public class TeamCreateOrEditViewModel
{
    public int Id { get; set; }
    
    [DisplayName("Όνομα Ομάδας")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    [DisplayName("Σύντομη περιγραφή")]
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ProfileImageUrl { get; set; }
}