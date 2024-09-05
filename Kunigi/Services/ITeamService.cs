using System.Security.Claims;
using Kunigi.ViewModels.Team;

namespace Kunigi.Services;

public interface ITeamService
{
    Task<List<TeamDetailsViewModel>> GetAllTeams();
    
    Task<TeamDetailsViewModel> GetTeamDetails(string teamSlug);
    
    Task CreateTeam(TeamCreateViewModel team);

    Task<TeamEditViewModel> PrepareEditTeamViewModel(string teamSlug, ClaimsPrincipal user);
    
    Task EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task<TeamMediaViewModel> GetTeamMedia(string teamSlug, ClaimsPrincipal user);

    Task AddTeamMedia(string teamSlug, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteTeamMedia(string teamSlug, int mediafileId, ClaimsPrincipal user);
}