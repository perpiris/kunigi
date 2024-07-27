﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.Data.Migrations;
using Kunigi.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Game;

public class GameCreateViewModel
{
    [DisplayName("Έτος διεξαγωγής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short? Year { get; set; }
    
    [DisplayName("Σειρά Διαεξαγωγής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short? Order { get; set; }
    
    [DisplayName("Διοργανωτής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int HostId { get; set; }
    
    [DisplayName("Νικητής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int WinnerId { get; set; }
    
    [DisplayName("Αφίσα")]
    public string ProfileImageUrl { get; set; }
    
    public SelectList HostSelectList { get; set; }
    
    public SelectList WinnerSelectList { get; set; }

    [DisplayName("Επιλογές")]
    public List<int> SelectedGameTypeIds { get; set; } = [];
    
    public List<GameType> GameTypes { get; set; } = [];
}