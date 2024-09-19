namespace Kunigi.ViewModels.Account;

public class AppUserDetailsViewModel
{
    public string Id { get; set; }

    public string Email { get; set; }
    
    public List<string> UserRoles { get; set; }
    
    public List<string> RoleList { get; set; }
}