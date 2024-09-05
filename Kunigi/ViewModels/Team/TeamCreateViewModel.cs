using System.ComponentModel.DataAnnotations;
using Kunigi.CustomAttributes;

namespace Kunigi.ViewModels.Team;

public class TeamCreateViewModel
{
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [StringLength(255)]
    [UniqueTeamName]
    public string Name { get; set; }
}