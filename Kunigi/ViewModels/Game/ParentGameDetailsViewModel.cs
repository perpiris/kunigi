namespace Kunigi.ViewModels.Game;

public class ParentGameDetailsViewModel
{
    public short Year { get; set; }

    public short Order { get; set; }
    
    public string Title { get; set; }

    public string Description { get; set; }
    
    public string Slug { get; set; }
    
    public string WinnerSlug { get; set; }

    public string Winner { get; set; }
    
    public string HostSlug { get; set; }
    
    public string Host { get; set; }
    
    public string ProfileImageUrl { get; set; }
    
    public List<GameDetailsViewModel> GameList { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
}