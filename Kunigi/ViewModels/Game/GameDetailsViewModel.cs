namespace Kunigi.ViewModels.Game;

public class GameDetailsViewModel
{
    public int Id { get; set; }
    
    public string Description { get; set; }

    public string Type { get; set; }
    
    public string Title { get; set; }

    public short Year { get; set; }

    public string Slug { get; set; }
}