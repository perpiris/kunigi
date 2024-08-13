using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Team;

public class TeamManagerUpdateViewModel
{
    public string Slug { get; set; }
    public string TeamName { get; set; }
    
    public string SelectedManagerId { get; set; }
    public SelectList ManagerSelectList { get; set; }
    
    public List<TeamManagerDetailsViewModel> ManagerList { get; set; }
}

public class TeamManagerDetailsViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
}