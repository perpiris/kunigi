using Kunigi.ViewModels.Common;

namespace Kunigi.ViewModels.Game;

public class ParentGameDetailsViewModel
{
    public Guid ParentGameId { get; set; }
    
    public short Year { get; set; }

    public short Order { get; set; }
    
    public string MainTitle { get; set; }
    
    public string SubTitle { get; set; }

    public string Description { get; set; }
    
    public string Slug { get; set; }
    
    public Guid WinnerId { get; set; }

    public string Winner { get; set; }
    
    public Guid HostId { get; set; }
    
    public string Host { get; set; }
    
    public string ProfileImageUrl { get; set; }
    
    public List<GameDetailsViewModel> GameList { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
}