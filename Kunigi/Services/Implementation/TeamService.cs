using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Exceptions;
using Kunigi.Mappings;
using Kunigi.Utilities;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Services.Implementation;

public class TeamService : ITeamService
{
    private readonly DataContext _context;
    private readonly IMediaService _mediaService;
    private readonly UserManager<AppUser> _userManager;

    public TeamService(DataContext context, IMediaService mediaService, UserManager<AppUser> userManager)
    {
        _context = context;
        _mediaService = mediaService;
        _userManager = userManager;
    }

    public async Task<PaginatedViewModel<TeamDetailsViewModel>> GetPaginatedTeams(int pageNumber, int pageSize)
    {
        var query = _context.Teams.AsQueryable();
        var totalItems = await query.CountAsync();

        var teams = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var teamViewModels = teams
            .Select(x => x.ToTeamDetailsViewModel())
            .ToList();

        return new PaginatedViewModel<TeamDetailsViewModel>
        {
            Items = teamViewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<TeamDetailsViewModel> GetTeamDetails(Guid teamId, ClaimsPrincipal user = null)
    {
        if (user is not null)
        {
            await CanEditTeam(teamId, user);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(teamId);
        }

        var teamDetails = await _context.Teams
            .Include(x => x.HostedGames.OrderBy(y => y.Year))
            .Include(x => x.WonGames.OrderBy(y => y.Year))
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.TeamId == teamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        return teamDetails.ToTeamDetailsViewModel(true);
    }

    public async Task CreateTeam(TeamCreateViewModel viewModel)
    {
        var slug = SlugGenerator.GenerateSlug(viewModel.Name.Trim());
        var newTeam = new Team
        {
            Name = viewModel.Name.Trim(),
            Slug = slug,
            IsActive = viewModel.IsActive,
            HostedGames = new List<ParentGame>(),
            WonGames = new List<ParentGame>(),
            MediaFiles = new List<TeamMedia>(),
            Managers = new List<TeamManager>()
        };

        _mediaService.CreateFolder($"teams/{slug}");

        _context.Teams.Add(newTeam);
        await _context.SaveChangesAsync();
    }

    public async Task<TeamEditViewModel> PrepareEditTeamViewModel(Guid teamId, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);

        var teamDetails = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        return teamDetails.ToTeamEditViewModel();
    }

    public async Task EditTeam(TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        await CanEditTeam(viewModel.TeamId, user);

        var teamDetails = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == viewModel.TeamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        teamDetails.CreatedYear = viewModel.CreatedYear;
        teamDetails.IsActive = viewModel.IsActive;
        teamDetails.Description = viewModel.Description?.Trim();
        teamDetails.Facebook = viewModel.Facebook?.Trim();
        teamDetails.Youtube = viewModel.Youtube?.Trim();
        teamDetails.Instagram = viewModel.Instagram?.Trim();
        teamDetails.Website = viewModel.Website?.Trim();

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"teams/{teamDetails.Slug.Trim()}");
            teamDetails.ProfileImagePath = profileImagePath;
        }

        _context.Teams.Update(teamDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<TeamManagerEditViewModel> PrepareTeamManagerEditViewModel(Guid teamId, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);

        var users = await _context.AppUsers.ToListAsync();
        var managerSelectList = new List<SelectListItem>
        {
            new() { Value = "", Text = "Επιλογή Χρήστη" }
        };

        managerSelectList.AddRange(users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Email
        }));

        var teamDetails = await _context.Teams
            .Include(x => x.Managers)
            .FirstOrDefaultAsync(x => x.TeamId == teamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        var viewModel = teamDetails.ToTeamManagerEditViewModel();
        viewModel.ManagerSelectList = new SelectList(managerSelectList, "Value", "Text");

        return viewModel;
    }

    public async Task AddTeamManager(TeamManagerEditViewModel viewModel, ClaimsPrincipal user)
    {
        await CanEditTeam(viewModel.TeamId, user);

        var selectedUser = await _context.AppUsers
            .FirstOrDefaultAsync(x => x.Id == viewModel.SelectedManagerId);

        var teamDetails = await _context.Teams
            .Include(x => x.Managers)
            .FirstOrDefaultAsync(x => x.TeamId == viewModel.TeamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        if (selectedUser != null)
        {
            var isInManagerRole = await _userManager.IsInRoleAsync(selectedUser, "Manager");

            if (!isInManagerRole)
            {
                var result = await _userManager.AddToRoleAsync(selectedUser, "Manager");
                if (!result.Succeeded)
                {
                    throw new Exception("Αποτυχία προσθήκης του χρήστη στο ρόλο του Διαχειριστή.");
                }
            }

            if (teamDetails.Managers.All(m => m.TeamManagerId.ToString() != selectedUser.Id))
            {
                var teamManager = new TeamManager
                {
                    TeamId = teamDetails.TeamId,
                    AppUserId = selectedUser.Id
                };

                _context.TeamManagers.Add(teamManager);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Αυτός ο χρήστης είναι ήδη διαχειριστής.");
            }
        }
        else
        {
            throw new NotFoundException("Ο χρήστης δεν βρέθηκε");
        }
    }

    public async Task RemoveTeamManager(Guid teamId, string managerId, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);

        var teamDetails = await _context.Teams
            .Include(x => x.Managers)
            .FirstOrDefaultAsync(x => x.TeamId == teamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        var selectedUser = teamDetails.Managers.SingleOrDefault(m => m.AppUserId == managerId);

        if (selectedUser != null)
        {
            teamDetails.Managers.Remove(selectedUser);
            var teamManager = await _context.TeamManagers
                .SingleOrDefaultAsync(tm => tm.TeamId == teamDetails.TeamId && tm.AppUserId == managerId);
            if (teamManager != null)
            {
                _context.TeamManagers.Remove(teamManager);
                await _context.SaveChangesAsync();
            }

            var isStillManagerInOtherTeams = await _context.TeamManagers
                .AnyAsync(tm => tm.AppUserId == managerId);

            if (!isStillManagerInOtherTeams)
            {
                var userToUpdate = await _userManager.FindByIdAsync(managerId);
                if (userToUpdate != null)
                {
                    await _userManager.RemoveFromRoleAsync(userToUpdate, "Manager");
                }
            }
        }
        else
        {
            throw new NotFoundException("Ο χρήστης δεν βρέθηκε");
        }
    }

    public async Task<TeamMediaViewModel> GetTeamMedia(Guid teamId, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);

        var teamDetails = await _context.Teams
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .Where(x => x.TeamId == teamId)
            .FirstOrDefaultAsync();

        var viewModel = teamDetails.ToTeamMediaViewModel();
        return viewModel;
    }

    public async Task AddTeamMedia(Guid teamId, List<IFormFile> files, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);

        var teamDetails = await _context.Teams
            .Include(x => x.MediaFiles)
            .Where(x => x.TeamId == teamId).FirstOrDefaultAsync();

        foreach (var file in files)
        {
            var filePath = await _mediaService.SaveMediaFile(file, $"teams/{teamDetails.Slug}");
            teamDetails.MediaFiles ??= new List<TeamMedia>();
            teamDetails.MediaFiles.Add(new TeamMedia
            {
                MediaFile = new MediaFile
                {
                    Path = filePath
                },
            });
        }

        _context.Teams.Update(teamDetails);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTeamMedia(Guid teamId, Guid mediafileId, ClaimsPrincipal user)
    {
        await CanEditTeam(teamId, user);
        await _mediaService.DeleteMediaFile(mediafileId);
    }

    private async Task CanEditTeam(Guid teamId, ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(teamId);
        ArgumentNullException.ThrowIfNull(user);

        var teamDetails = await _context.Teams
            .Include(team => team.Managers)
            .FirstOrDefaultAsync(x => x.TeamId == teamId);

        if (teamDetails is null)
        {
            throw new NotFoundException("Η ομάδα δεν βρέθηκε");
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (teamDetails.Managers.All(x => x.AppUserId != userId))
            {
                throw new UnauthorizedOperationException("Δεν έχετε δικαίωμα επεξεργασίας.");
            }
        }
    }
}