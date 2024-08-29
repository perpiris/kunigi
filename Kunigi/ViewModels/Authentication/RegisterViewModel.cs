using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Authentication;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [DataType(DataType.Password)]
    [DisplayName("Κωδικός")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [DataType(DataType.Password)]
    [DisplayName("Επαλήθευση Κωδικού")]
    [Compare("Password", ErrorMessage = "Ο κωδικός δεν ταιριάζει.")]
    public string ConfirmPassword { get; set; }
}