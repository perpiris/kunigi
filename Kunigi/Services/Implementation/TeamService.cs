using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Exceptions;
using Kunigi.Mappings;
using Kunigi.Utilities;
using Kunigi.ViewModels.Team;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Services.Implementation;

public class TeamService : ITeamService
{
    private readonly DataContext _context;
    private readonly IMediaService _mediaService;

    public TeamService(DataContext context, IMediaService mediaService)
    {
        _context = context;
        _mediaService = mediaService;
    }

    public async Task<List<TeamDetailsViewModel>> GetAllTeams()
    {
        var allTeams = await _context.Teams.ToListAsync();
        return allTeams.Select(x => x.ToBaseTeamDetailsViewModel()).ToList();
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

        return teamDetails.ToFullTeamDetailsViewModel();
    }

    public async Task CreateTeam(TeamCreateViewModel teamData)
    {
        var slug = SlugGenerator.GenerateSlug(teamData.Name);
        var newTeam = new Team
        {
            Name = teamData.Name,
            Slug = slug
        };

        var teamFolderPath = _mediaService.CreateFolder($"teams/{slug}");
        newTeam.TeamFolderPath = teamFolderPath;

        _context.Teams.Add(newTeam);
        await _context.SaveChangesAsync();
    }

    public async Task EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        var teamDetails = await CheckTeamAndOwneship(teamSlug, user);

        teamDetails.Description = viewModel.Description;
        teamDetails.Facebook = viewModel.Facebook;
        teamDetails.Youtube = viewModel.Youtube;
        teamDetails.Instagram = viewModel.Instagram;
        teamDetails.Website = viewModel.Website;

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"teams/{teamDetails.Slug}", true);
            teamDetails.TeamProfileImagePath = profileImagePath;
        }

        _context.Teams.Update(teamDetails);
        await _context.SaveChangesAsync();
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