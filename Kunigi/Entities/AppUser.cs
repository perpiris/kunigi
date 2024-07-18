using Microsoft.AspNetCore.Identity;

namespace Kunigi.Entities;

public class AppUser : IdentityUser
{
    public virtual ICollection<Team> ManagedTeams { get; set; }
}