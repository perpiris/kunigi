namespace Kunigi.ViewModels.Game;

public class GameDetailsViewModel
{
    public Guid GameId { get; set; }
    
    public short Year { get; set; }
    
    public string GameType { get; set; }

    public string TypeSlug { get; set; }
    
    public string ParentGameTitle { get; set; }
    
    public string GameTitle { get; set; }

    public string Description { get; set; }
}