using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Exceptions;
using Kunigi.Mappings;
using Kunigi.ViewModels.Game;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Services.Implementation;

public class GameService : IGameService
{
    private readonly DataContext _context;
    private readonly IMediaService _mediaService;

    public GameService(DataContext context, IMediaService mediaService)
    {
        _context = context;
        _mediaService = mediaService;
    }

    public async Task<List<ParentGameDetailsViewModel>> GetAllGames()
    {
        var allTeams = await _context.ParentGames.ToListAsync();
        return allTeams.Select(x => x.ToBaseParentGameDetailsViewModel()).ToList();
    }

    public async Task<ParentGameDetailsViewModel> GetParentGameDetails(short gameYear)
    {
        if (gameYear <= default(short))
        {
            throw new ArgumentNullException(nameof(gameYear));
        }

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Host)
            .Include(x => x.Winner)
            .Include(x => x.Games)
            .ThenInclude(x => x.GameType)
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.Year == gameYear);

        if (parentGameDetails is null)
        {
            throw new NotFoundException();
        }

        return parentGameDetails.ToFullParentGameDetailsViewModel();
    }

    public async Task<GameDetailsViewModel> GetGameDetails(short gameYear, string gameTypeSlug)
    {
        if (gameYear <= default(short))
        {
            throw new ArgumentNullException(nameof(gameYear));
        }

        if (string.IsNullOrEmpty(gameTypeSlug))
        {
            throw new ArgumentNullException(nameof(gameTypeSlug));
        }

        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .Include(x => x.GameType)
            .FirstOrDefaultAsync(x => x.ParentGame.Year == gameYear && x.GameType.Slug == gameTypeSlug.Trim());

        return gameDetails.ToBaseGameDetailsViewModel();
    }

    public async Task<ParentGameCreateViewModel> PrepareCreateParentGameViewModel(ParentGameCreateViewModel viewModel)
    {
        viewModel ??= new ParentGameCreateViewModel();

        var teamList = await _context.Teams.ToListAsync();
        var gameTypes = await _context.GameTypes.ToListAsync();
        teamList.Insert(0, new Team { TeamId = 0, Name = "Επιλογή Ομάδας" });

        viewModel.HostSelectList = new SelectList(teamList, "TeamId", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "TeamId", "Name", 0);

        viewModel.GameTypes = gameTypes;

        viewModel.Year = (short)DateTime.Now.Year;
        viewModel.Order = 1;

        return viewModel;
    }

    public async Task CreateParentGame(ParentGameCreateViewModel viewModel)
    {
        var slug = viewModel.Year.ToString();
        var newParentGame = new ParentGame
        {
            Year = viewModel.Year,
            Order = viewModel.Order,
            Slug = slug,
            Title = "Τίτλος",
            Description = "Περιγραφή",
            WinnerId = viewModel.WinnerId,
            HostId = viewModel.HostId,
            Games = new List<Game>()
        };

        foreach (var gameTypeId in viewModel.SelectedGameTypeIds)
        {
            newParentGame.Games.Add(new Game
            {
                GameTypeId = gameTypeId
            });
        }

        var gameFolderPath = _mediaService.CreateFolder($"games/{slug}");
        newParentGame.ParentGameFolderPath = gameFolderPath;

        _context.ParentGames.Add(newParentGame);
        await _context.SaveChangesAsync();
    }

    public Task<ParentGameEditViewModel> PrepareEditParentGameViewModel(short gameYear, ClaimsPrincipal user)
    {
        throw new NotImplementedException();
    }

    public async Task EditParentGame(short gameYear, ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);

        parentGameDetails.Title = viewModel.Title;
        parentGameDetails.Description = viewModel.Description;

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"games/{parentGameDetails.Slug}", true);
            parentGameDetails.ParentGameProfileImagePath = profileImagePath;
        }

        _context.ParentGames.Update(parentGameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<GameMediaViewModel> GetTeamMedia(short gameYear, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        var parentGameMedia = await _context.ParentGameMediaFiles
            .Include(x => x.MediaFile)
            .Where(x => x.ParentGameId == parentGameDetails.ParentGameId).ToListAsync();

        var viewModel = GameMappings.ToGameMediaViewModel(gameYear, parentGameMedia);
        return viewModel;
    }

    public async Task AddGameMedia(short gameYear, List<IFormFile> files, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);

        foreach (var file in files)
        {
            var filePath = await _mediaService.SaveMediaFile(file, $"games/{parentGameDetails.Year}", false);
            parentGameDetails.MediaFiles ??= new List<ParentGameMedia>();
            parentGameDetails.MediaFiles.Add(new ParentGameMedia
            {
                MediaFile = new MediaFile
                {
                    Path = filePath
                },
            });
        }

        _context.ParentGames.Update(parentGameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGameMedia(short gameYear, int mediafileId, ClaimsPrincipal user)
    {
        await CheckGameAndOwneship(gameYear, user);
        await _mediaService.DeleteMediaFile(mediafileId);
    }

    private async Task<ParentGame> CheckGameAndOwneship(short gameYear, ClaimsPrincipal user)
    {
        if (gameYear <= default(short))
        {
            throw new ArgumentNullException(nameof(gameYear));
        }

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Host)
            .ThenInclude(x => x.Managers)
            .FirstOrDefaultAsync(x => x.Year == gameYear);

        if (parentGameDetails is null)
        {
            throw new NotFoundException();
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (parentGameDetails.Host.Managers.All(x => x.Id != userId))
            {
                throw new UnauthorizedOperationException();
            }
        }

        return parentGameDetails;
    }
}