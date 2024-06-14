namespace Kunigi.ViewModels.Game;

public class GameDetailsViewModel
{
    public int Id { get; set; }
    
    public short Year { get; set; }
    
    public string Title { get; set; }

    public string Winner { get; set; }

    public string Host { get; set; }
    
    public string Description { get; set; }
    
    public string ImageUrl { get; set; }
}