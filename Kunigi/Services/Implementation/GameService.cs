using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Enums;
using Kunigi.Exceptions;
using Kunigi.Mappings;
using Kunigi.ViewModels.Common;
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

    public async Task<PaginatedViewModel<ParentGameDetailsViewModel>> GetPaginatedGame(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.ParentGames.AsQueryable();
        var totalItems = await query.CountAsync();

        var games = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var gameViewModels = games.Select(x => x.ToParentGameDetailsViewModel())
            .OrderByDescending(x => x.Year).ToList();

        return new PaginatedViewModel<ParentGameDetailsViewModel>
        {
            Items = gameViewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ParentGameDetailsViewModel> GetParentGameDetails(short gameYear, ClaimsPrincipal user = null)
    {
        ParentGame parentGameDetails;
        if (user is not null)
        {
            parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        }
        else
        {
            if (gameYear <= default(short))
            {
                throw new ArgumentNullException(nameof(gameYear));
            }

            parentGameDetails = await GetFullParentGameDetails(gameYear);
        }

        if (parentGameDetails is null)
        {
            throw new NotFoundException();
        }

        return parentGameDetails.ToParentGameDetailsViewModel(true);
    }

    public async Task<GameDetailsViewModel> GetGameDetails(short gameYear, string gameTypeSlug, ClaimsPrincipal user = null)
    {
        ParentGame parentGameDetails;
        if (user is not null)
        {
            parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        }
        else
        {
            if (gameYear <= default(short))
            {
                throw new ArgumentNullException(nameof(gameYear));
            }
            
            if (string.IsNullOrEmpty(gameTypeSlug))
            {
                throw new ArgumentNullException(nameof(gameTypeSlug));
            }
            
            parentGameDetails = await GetFullParentGameDetails(gameYear);
        }
        
        var gameDetails = parentGameDetails.Games.SingleOrDefault(x => x.GameType.Slug == gameTypeSlug);
        return gameDetails.ToGameDetailsViewModel();
    }

    public async Task<GamePuzzleDetailsViewModel> GetGamePuzzleList(short gameYear, string gameTypeSlug)
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
            .Include(x => x.GameType)
            .Include(x => x.ParentGame)
            .Include(x => x.PuzzleList)
            .ThenInclude(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.GameType.Slug == gameTypeSlug && x.ParentGame.Year == gameYear);

        if (gameDetails == null)
        {
            throw new NotFoundException();
        }

        return gameDetails.ToGamePuzzleDetailsViewModel();
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
            MainTitle = $"{viewModel.Order}ο Κυνήγι Θησαυρού",
            WinnerId = viewModel.WinnerId,
            HostId = viewModel.HostId,
            Games = new List<Game>()
        };

        foreach (var gameTypeId in viewModel.SelectedGameTypeIds)
        {
            newParentGame.Games.Add(new Game
            {
                GameTypeId = gameTypeId,
                Description = "Δεν υπάρχουν πληροφορίες"
            });
        }

        var gameFolderPath = _mediaService.CreateFolder($"games/{slug}");
        newParentGame.ParentGameFolderPath = gameFolderPath;

        _context.ParentGames.Add(newParentGame);
        await _context.SaveChangesAsync();
    }

    public async Task<ParentGameEditViewModel> PrepareParentGameEditViewModel(short gameYear, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        return parentGameDetails.ToParentGameEditViewModel();
    }
    
    public async Task<GameEditViewModel> PrepareGameEditViewModel(short gameYear, string gameTypeSlug, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        var gameDetails = parentGameDetails.Games.FirstOrDefault(x => x.GameType.Slug == gameTypeSlug.Trim());
        return gameDetails.ToGameEditViewModel();
    }

    public async Task EditParentGame(short gameYear, ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);

        parentGameDetails.SubTitle = viewModel.SubTitle;
        parentGameDetails.Description = viewModel.Description;

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"games/{parentGameDetails.Slug}", true);
            parentGameDetails.ParentGameProfileImagePath = profileImagePath;
        }

        _context.ParentGames.Update(parentGameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task EditGame(short gameYear, string gameTypeSlug, GameEditViewModel viewModel, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        var gameDetails = parentGameDetails.Games.FirstOrDefault(x => x.GameType.Slug == gameTypeSlug.Trim());
        
        if (gameDetails == null)
        {
            throw new NotFoundException();
        }
        
        gameDetails.Description = viewModel.Description;
        
        _context.Games.Update(gameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<ParentGameMediaViewModel> GetParentGameMEdia(short gameYear, ClaimsPrincipal user)
    {
        var parentGameDetails = await CheckGameAndOwneship(gameYear, user);
        var viewModel = parentGameDetails.ToParentGameMediaViewModel();
        return viewModel;
    }

    public async Task AddParentGameMedia(short gameYear, List<IFormFile> files, ClaimsPrincipal user)
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

    public async Task DeleteParentGameMedia(short gameYear, int mediafileId, ClaimsPrincipal user)
    {
        await CheckGameAndOwneship(gameYear, user);
        await _mediaService.DeleteMediaFile(mediafileId);
    }

    public async Task<GamePuzzleCreateViewModel> PrepareCreateGamePuzzleViewModel(short gameYear, string gameTypeSlug)
    {
        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .Include(x => x.GameType)
            .Include(x => x.PuzzleList)
            .FirstOrDefaultAsync(x => x.GameType.Slug == gameTypeSlug && x.ParentGame.Year == gameYear);
        
        var maxOrder = gameDetails.PuzzleList.Max(x => (short?)x.Order) ?? 0;
        
        var viewModel = gameDetails.ToGamePuzzleCreateViewModel((short)(maxOrder + 1));
        return viewModel;
    }

    public async Task CreateGamePuzzle(GamePuzzleCreateViewModel viewModel)
    {
        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .Include(x => x.PuzzleList)
            .FirstOrDefaultAsync(x => viewModel.GameId == x.GameId);
        
        if (gameDetails == null)
        {
            throw new NotFoundException();
        }

        var maxOrder = gameDetails.PuzzleList.Max(x => (short?)x.Order) ?? 0;
        
        var puzzle = new Puzzle
        {
            Game = gameDetails,
            Question = viewModel.Question,
            Answer = viewModel.Answer,
            Order = maxOrder,
            Group = viewModel.Group != maxOrder ? viewModel.Group : maxOrder,
            MediaFiles = new List<PuzzleMedia>()
        };

        if (viewModel.QuestionMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.QuestionMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{gameDetails.ParentGame.Year}", false);
                puzzle.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = PuzzleMediaType.Question,
                    MediaFile = new MediaFile
                    {
                        Path = mediaFilePath
                    }
                });
            }
        }

        if (viewModel.AnswerMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.AnswerMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{gameDetails.ParentGame.Year}", false);
                puzzle.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = PuzzleMediaType.Answer,
                    MediaFile = new MediaFile
                    {
                        Path = mediaFilePath
                    }
                });
            }
        }

        _context.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();
    }
    
    public async Task<GamePuzzleEditViewModel> PrepareEditGamePuzzleViewModel(int puzzleId)
    {
        var puzzleDetails = await _context.Puzzles
            .Include(x => x.Game.GameType)
            .Include(x => x.Game.ParentGame)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);
        
        var viewModel = puzzleDetails.ToGamePuzzleEditViewModel();
        return viewModel;
    }

    public async Task EditGamePuzzle(GamePuzzleEditViewModel viewModel)
    {
        var puzzleDetails = await _context.Puzzles
            .Include(x => x.Game)
            .ThenInclude(x => x.ParentGame)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.PuzzleId == viewModel.PuzzleId);

        puzzleDetails.Question = viewModel.Question;
        puzzleDetails.Answer = viewModel.Answer;
        puzzleDetails.Group = viewModel.Group is > 0 ? viewModel.Group.Value : puzzleDetails.Order;
        
        if (viewModel.QuestionMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.QuestionMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{puzzleDetails.Game.ParentGame.Year}", false);
                puzzleDetails.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = PuzzleMediaType.Question,
                    MediaFile = new MediaFile
                    {
                        Path = mediaFilePath
                    }
                });
            }
        }

        if (viewModel.AnswerMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.AnswerMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{puzzleDetails.Game.ParentGame.Year}", false);
                puzzleDetails.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = PuzzleMediaType.Answer,
                    MediaFile = new MediaFile
                    {
                        Path = mediaFilePath
                    }
                });
            }
        }
        
        _context.Puzzles.Update(puzzleDetails);
        await _context.SaveChangesAsync();
    }

    private async Task<ParentGame> CheckGameAndOwneship(short gameYear, ClaimsPrincipal user)
    {
        if (gameYear <= default(short))
        {
            throw new ArgumentNullException(nameof(gameYear));
        }
        
        ArgumentNullException.ThrowIfNull(user);

        var parentGameDetails = await GetFullParentGameDetails(gameYear);
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

    private async Task<ParentGame> GetFullParentGameDetails(short gameYear)
    {
        return await _context.ParentGames
            .Include(x => x.Host)
            .ThenInclude(team => team.Managers)
            .Include(x => x.Winner)
            .Include(x => x.Games)
            .ThenInclude(x => x.GameType)
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.Year == gameYear);
    }
}