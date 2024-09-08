using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;

namespace Kunigi.ViewModels.Team;

public class TeamDetailsViewModel
{
    public string Name { get; set; }
    
    public string TeamSlug { get; set; }
    
    public short? CreatedYear { get; set; }
    
    public bool IsActive { get; set; }
    
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    public string ProfileImageUrl { get; set; }

    public List<ParentGameDetailsViewModel> GamesWon { get; set; }
    
    public List<ParentGameDetailsViewModel> GamesHosted { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
}