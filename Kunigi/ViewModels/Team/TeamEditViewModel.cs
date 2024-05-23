using System.ComponentModel;

namespace Kunigi.ViewModels.Team;

public class TeamEditViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    [DisplayName("Σύντομη περιγραφή")]
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ImageUrl { get; set; }
}