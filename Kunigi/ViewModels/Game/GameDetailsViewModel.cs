namespace Kunigi.ViewModels.Game;

public class GameDetailsViewModel
{
    public int Id { get; set; }
    
    public short Year { get; set; }

    public short Order { get; set; }
    
    public string Title { get; set; }
    
    public int WinnerId { get; set; }

    public string Winner { get; set; }
    
    public int HostId { get; set; }
    
    public string Host { get; set; }
    
    public string ProfileImageUrl { get; set; }
    
    public List<SubGameDetailsViewModel> SubGamesList { get; set; }
}