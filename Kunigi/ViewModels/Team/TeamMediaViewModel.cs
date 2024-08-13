namespace Kunigi.ViewModels.Team;

public class TeamMediaViewModel
{
    public string TeamSlug { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
    
    public List<IFormFile> NewMediaFiles { get; set; }
}