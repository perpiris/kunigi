namespace Kunigi.ViewModels.Game;

public class ParentGameDetailsViewModel
{
    public int Id { get; set; }
    
    public short Year { get; set; }

    public short Order { get; set; }
    
    public string Title { get; set; }
    
    public string WinnerSlug { get; set; }

    public string Winner { get; set; }
    
    public string HostSlug { get; set; }
    
    public string Host { get; set; }
    
    public string ProfileImageUrl { get; set; }
    
    public List<GameDetailsViewModel> GameList { get; set; }
    
    public string GetFullTitle()
    {
        return $"{Title} ({Year})";
    }
}