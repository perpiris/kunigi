using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Game;

public class ParentGameCreateViewModel
{
    [DisplayName("Έτος διεξαγωγής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short Year { get; set; }

    [DisplayName("Σειρά")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public short Order { get; set; }
    
    [DisplayName("Διοργανωτής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public Guid HostId { get; set; }
    
    [DisplayName("Νικητής")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public Guid WinnerId { get; set; }
    
    [DisplayName("Επιλογές")]
    public List<Guid> SelectedGameTypeIds { get; set; } = [];
    
    public SelectList HostSelectList { get; set; }
    
    public SelectList WinnerSelectList { get; set; }
    
    public List<GameType> GameTypes { get; set; } = [];
}