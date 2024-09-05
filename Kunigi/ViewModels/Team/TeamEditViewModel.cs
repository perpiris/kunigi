using System.ComponentModel;

namespace Kunigi.ViewModels.Team;

public class TeamEditViewModel
{
    public int TeamId { get; set; }
    
    [DisplayName("Όνομα Ομάδας")]
    public string Name { get; set; }
    
    [DisplayName("Σύντομη περιγραφή")]
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ProfileImageUrl { get; set; }
}