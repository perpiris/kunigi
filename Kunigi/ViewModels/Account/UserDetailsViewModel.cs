namespace Kunigi.ViewModels.Account;

public class UserDetailsViewModel
{
    public string Id { get; set; }

    public string Email { get; set; }
    
    public List<string> Roles { get; set; }
}