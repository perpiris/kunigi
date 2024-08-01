namespace Kunigi.ViewModels.Account;

public class UserDetailsUpdateViewModel
{
    public string Id { get; set; }

    public string Email { get; set; }
    
    public List<string> UserRoles { get; set; }
    
    public List<string> RoleList { get; set; }
}