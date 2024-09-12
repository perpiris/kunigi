﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Game;

public class GameEditViewModel
{
    [DisplayName("Τίτλος")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Title { get; set; }

    [DisplayName("Περιγραφή")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Description { get; set; }
    
    public string GameType { get; set; }
    
    public string GameTypeSlug { get; set; }
    
    public short GameYear { get; set; }
}