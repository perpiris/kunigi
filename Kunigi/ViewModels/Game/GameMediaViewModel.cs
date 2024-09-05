﻿namespace Kunigi.ViewModels.Game;

public class GameMediaViewModel
{
    public short Year { get; set; }
    
    public List<MediaFileViewModel> MediaFiles { get; set; } = [];
    
    public List<IFormFile> NewMediaFiles { get; set; } // do not delete
}