using Kunigi.ViewModels.Common;

namespace Kunigi.ViewModels.Game;

public class ParentGameMediaViewModel
{
    public string YearSlug { get; set; }
    
    public string Ttitle { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
    
    public List<IFormFile> NewMediaFiles { get; set; }
}