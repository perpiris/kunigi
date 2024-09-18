using System.Security.Claims;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Team;

namespace Kunigi.Services;

public interface ITeamService
{
    Task<PaginatedViewModel<TeamDetailsViewModel>> GetPaginatedTeams(int pageNumber, int pageSize);
    
    Task<TeamDetailsViewModel> GetTeamDetails(Guid teamId, ClaimsPrincipal user = null);
    
    Task CreateTeam(TeamCreateViewModel viewModel);

    Task<TeamEditViewModel> PrepareEditTeamViewModel(Guid teamId, ClaimsPrincipal user);
    
    Task EditTeam(TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task<TeamManagerEditViewModel> PrepareTeamManagerEditViewModel(Guid teamId, ClaimsPrincipal user);
    
    Task AddTeamManager(TeamManagerEditViewModel viewModel, ClaimsPrincipal user);
    
    Task RemoveTeamManager(Guid teamId, string managerId, ClaimsPrincipal user);
    
    Task<TeamMediaViewModel> GetTeamMedia(Guid teamId, ClaimsPrincipal user);

    Task AddTeamMedia(Guid teamId, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteTeamMedia(Guid teamId, Guid mediafileId, ClaimsPrincipal user);
}