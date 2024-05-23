using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Team;

public class TeamCreateViewModel
{
    [DisplayName("Όνομα Ομάδας")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    public string Name { get; set; }
}