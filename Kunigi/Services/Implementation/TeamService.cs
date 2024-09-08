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

        var teamViewModels = teams.Select(x => x.ToTeamDetailsViewModel()).ToList();

        return new PaginatedViewModel<TeamDetailsViewModel>
        {
            Items = teamViewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<TeamDetailsViewModel> GetTeamDetails(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            throw new ArgumentNullException(nameof(teamSlug));
        }

        teamSlug = teamSlug.Trim();
        var teamDetails = await _context.Teams
            .Include(x => x.HostedGames.OrderBy(y => y.Year))
            .Include(x => x.WonGames.OrderBy(y => y.Year))
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.Slug == teamSlug);

        if (teamDetails is null)
        {
            throw new NotFoundException();
        }

        return teamDetails.ToTeamDetailsViewModel(true);
    }

    public async Task CreateTeam(TeamCreateViewModel viewModel)
    {
        var slug = SlugGenerator.GenerateSlug(viewModel.Name);
        var newTeam = new Team
        {
            Name = viewModel.Name.Trim(),
            Description = "Δεν υπάρχει περιγραφή.",
            Slug = slug,
            IsActive = viewModel.IsActive
        };

        var teamFolderPath = _mediaService.CreateFolder($"teams/{slug}");
        newTeam.TeamFolderPath = teamFolderPath;

        _context.Teams.Add(newTeam);
        await _context.SaveChangesAsync();
    }

    public async Task EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        teamSlug = teamSlug.Trim();
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);

        teamDetails.CreatedYear = viewModel.CreatedYear;
        teamDetails.IsActive = viewModel.IsActive;
        teamDetails.Description = viewModel.Description?.Trim();
        teamDetails.Facebook = viewModel.Facebook?.Trim();
        teamDetails.Youtube = viewModel.Youtube?.Trim();
        teamDetails.Instagram = viewModel.Instagram?.Trim();
        teamDetails.Website = viewModel.Website?.Trim();

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"teams/{teamDetails.Slug.Trim()}", true);
            teamDetails.TeamProfileImagePath = profileImagePath;
        }

        _context.Teams.Update(teamDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<TeamManagerEditViewModel> PrepareTeamManagerEditViewModel(string teamSlug, ClaimsPrincipal user)
    {
        teamSlug = teamSlug.Trim();
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);

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

        var viewModel = teamDetails.ToTeamManagerEditViewModel();
        viewModel.ManagerSelectList = new SelectList(managerSelectList, "Value", "Text");

        return viewModel;
    }

    public async Task AddTeamManager(TeamManagerEditViewModel viewModel, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(viewModel.TeamSlug, user);

        var selectedUser = await _context.AppUsers
            .SingleOrDefaultAsync(x => x.Id == viewModel.SelectedManagerId);

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

            if (teamDetails.Managers.All(m => m.Id != selectedUser.Id))
            {
                teamDetails.Managers.Add(selectedUser);

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
            throw new NotFoundException();
        }
    }

    public async Task RemoveTeamManager(string teamSlug, string managerId, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);

        var selectedUser = teamDetails.Managers.SingleOrDefault(m => m.Id == managerId);
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
            throw new NotFoundException();
        }
    }

    public async Task<TeamEditViewModel> PrepareEditTeamViewModel(string teamSlug, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);
        return teamDetails.ToTeamEditViewModel();
    }

    public async Task<TeamMediaViewModel> GetTeamMedia(string teamSlug, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);
        var teamMedia = await _context.TeamMediaFiles
            .Include(x => x.MediaFile)
            .Where(x => x.TeamId == teamDetails.TeamId).ToListAsync();

        var viewModel = TeamMappings.ToTeamMediaViewModel(teamDetails, teamMedia);
        return viewModel;
    }

    public async Task AddTeamMedia(string teamSlug, List<IFormFile> files, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);

        foreach (var file in files)
        {
            var filePath = await _mediaService.SaveMediaFile(file, $"teams/{teamDetails.Slug}", false);
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

    public async Task DeleteTeamMedia(string teamSlug, int mediafileId, ClaimsPrincipal user)
    {
        await CheckTeamAndOwneship(teamSlug, user);
        await _mediaService.DeleteMediaFile(mediafileId);
    }

    private async Task<Team> CheckTeamAndOwneship(string teamSlug, ClaimsPrincipal user)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            throw new ArgumentNullException(nameof(teamSlug));
        }

        teamSlug = teamSlug.Trim();
        var teamDetails = await _context.Teams
            .Include(team => team.Managers)
            .FirstOrDefaultAsync(x => x.Slug == teamSlug);

        if (teamDetails is null)
        {
            throw new NotFoundException();
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (teamDetails.Managers.All(x => x.Id != userId))
            {
                throw new UnauthorizedOperationException();
            }
        }

        return teamDetails;
    }
}