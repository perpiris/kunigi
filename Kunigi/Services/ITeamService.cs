using System.Security.Claims;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Team;

namespace Kunigi.Services;

public interface ITeamService
{
    Task<PaginatedViewModel<TeamDetailsViewModel>> GetPaginatedTeams(int pageNumber, int pageSize);
    
    Task<TeamDetailsViewModel> GetTeamDetails(string teamSlug, ClaimsPrincipal user = null);
    
    Task CreateTeam(TeamCreateViewModel viewModel);

    Task<TeamEditViewModel> PrepareEditTeamViewModel(string teamSlug, ClaimsPrincipal user);
    
    Task EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task<TeamManagerEditViewModel> PrepareTeamManagerEditViewModel(string teamSlug, ClaimsPrincipal user);
    
    Task AddTeamManager(TeamManagerEditViewModel viewModel, ClaimsPrincipal user);
    
    Task RemoveTeamManager(string teamSlug, string managerId, ClaimsPrincipal user);
    
    Task<TeamMediaViewModel> GetTeamMedia(string teamSlug, ClaimsPrincipal user);

    Task AddTeamMedia(string teamSlug, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteTeamMedia(string teamSlug, int mediafileId, ClaimsPrincipal user);
}