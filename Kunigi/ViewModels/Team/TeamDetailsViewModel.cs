using Kunigi.ViewModels.Game;

namespace Kunigi.ViewModels.Team;

public class TeamDetailsViewModel
{
    public int TeamId { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
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