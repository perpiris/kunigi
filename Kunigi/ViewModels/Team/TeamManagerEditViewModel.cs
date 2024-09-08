using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Team;

public class TeamManagerEditViewModel
{
    public string TeamSlug { get; set; }
    
    public string Name { get; set; }
    
    public string SelectedManagerId { get; set; }
    
    public SelectList ManagerSelectList { get; set; }
    
    public List<TeamManagerDetailsViewModel> ManagerList { get; set; }
}