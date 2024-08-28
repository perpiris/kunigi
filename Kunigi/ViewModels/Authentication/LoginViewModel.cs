using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Το πεδίο απαιτείται.")]
    [DataType(DataType.Password)]
    [DisplayName("Κωδικός")]
    public string Password { get; set; }
}