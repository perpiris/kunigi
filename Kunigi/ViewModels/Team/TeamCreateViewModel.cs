using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.CustomAttributes;

namespace Kunigi.ViewModels.Team;

public class TeamCreateViewModel
{
    [DisplayName("Όνομα Ομάδας")]
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [StringLength(255)]
    [UniqueTeamName]
    public string Name { get; set; }

    [DisplayName("Ενεργή")] public bool IsActive { get; set; } = true;
}