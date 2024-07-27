using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Game;

public class GameCreateOrUpdateViewModel
{
    public int Id { get; set; }
    
    [DisplayName("Έτος διεξαγωγής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short Year { get; set; }

    [DisplayName("Σειρά")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short Order { get; set; }
    
    [DisplayName("Διοργανωτής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int HostId { get; set; }
    
    [DisplayName("Νικητής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public int WinnerId { get; set; }
    
    [DisplayName("Τίτλος")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Title { get; set; }

    [DisplayName("Περιγραφή")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Description { get; set; }
    
    [DisplayName("Αφίσα")]
    public string ProfileImageUrl { get; set; }
    
    public SelectList HostSelectList { get; set; }
    
    public SelectList WinnerSelectList { get; set; }
    
    [DisplayName("Επιλογές")]
    public List<int> SelectedGameTypeIds { get; set; } = [];
    
    public List<GameType> GameTypes { get; set; } = [];
}