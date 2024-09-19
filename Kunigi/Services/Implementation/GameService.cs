using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Exceptions;
using Kunigi.Mappings;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    public async Task<PaginatedViewModel<ParentGameDetailsViewModel>> GetPaginatedParentGames(int pageNumber = 1, int pageSize = 10)
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

    public async Task<ParentGameDetailsViewModel> GetParentGameDetails(Guid parentGameId, ClaimsPrincipal user = null)
    {
        if (user is not null)
        {
            await CanEditParentGame(parentGameId, user);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(parentGameId);
        }

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Host)
            .ThenInclude(team => team.Managers)
            .Include(x => x.Winner)
            .Include(x => x.Games)
            .ThenInclude(x => x.GameType)
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.ParentGameId == parentGameId);

        if (parentGameDetails is null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        return parentGameDetails.ToParentGameDetailsViewModel(true);
    }

    public async Task<GameDetailsViewModel> GetGameDetails(Guid gameId, ClaimsPrincipal user = null)
    {
        if (user is not null)
        {
            await CanEditGame(gameId, user);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(gameId);
        }

        var gameDetails = await _context.Games
            .Include(x => x.GameType)
            .FirstOrDefaultAsync(x => x.GameId == gameId);

        if (gameDetails is null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        return gameDetails.ToGameDetailsViewModel();
    }

    public async Task<GamePuzzleDetailsViewModel> GetGamePuzzleList(Guid gameId)
    {
        ArgumentNullException.ThrowIfNull(gameId);

        var gameDetails = await _context.Games
            .Include(x => x.GameType)
            .Include(x => x.ParentGame)
            .Include(x => x.PuzzleList)
            .ThenInclude(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.GameId == gameId);

        if (gameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        return gameDetails.ToGamePuzzleDetailsViewModel();
    }

    public async Task<ParentGameCreateViewModel> PrepareCreateParentGameViewModel(ParentGameCreateViewModel viewModel)
    {
        viewModel ??= new ParentGameCreateViewModel();

        var teamList = await _context.Teams.ToListAsync();
        var gameTypes = await _context.GameTypes.ToListAsync();
        teamList.Insert(0, new Team { TeamId = Guid.Empty, Name = "Επιλογή Ομάδας" });

        viewModel.HostSelectList = new SelectList(teamList, "TeamId", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "TeamId", "Name", 0);

        viewModel.GameTypes = gameTypes;
        viewModel.Year = (short)DateTime.Now.Year;
        viewModel.Order = (short)(DateTime.Now.Year - 1990);

        return viewModel;
    }

    public async Task CreateParentGame(ParentGameCreateViewModel viewModel, ModelStateDictionary modelState)
    {
        if (!(viewModel.HostId != Guid.Empty))
        {
            modelState.AddModelError("HostId", "Το πεδίο απαιτείται.");
        }
        else
        {
            modelState.Remove("HostId");
        }

        if (!(viewModel.WinnerId != Guid.Empty))
        {
            modelState.AddModelError("WinnerId", "Το πεδίο απαιτείται.");
        }
        else
        {
            modelState.Remove("WinnerId");
        }

        var parentGameList = await _context.ParentGames.ToListAsync();

        var yearExists = parentGameList.Any(x => x.Year == viewModel.Year);
        if (yearExists)
        {
            modelState.AddModelError("Year", "Υπάρχει ήδη παιχνίδι για αυτή τη χρονιά.");
        }
        else
        {
            modelState.Remove("Year");
        }

        var orderExists = parentGameList.Any(x => x.Order == viewModel.Order);
        if (orderExists)
        {
            modelState.AddModelError("Order", "Υπάρχει ήδη παιχνίδι για αυτή τη σειρά.");
        }
        else
        {
            modelState.Remove("Order");
        }

        if (!modelState.IsValid)
        {
            throw new InvalidFormException();
        }

        var slug = viewModel.Year.ToString();
        var newParentGame = new ParentGame
        {
            Year = viewModel.Year,
            Order = viewModel.Order,
            Slug = slug,
            MainTitle = $"{viewModel.Order}ο Κυνήγι Θησαυρού",
            WinnerId = viewModel.WinnerId,
            HostId = viewModel.HostId,
            Games = new List<Game>(),
            MediaFiles = new List<ParentGameMedia>()
        };

        var gameTypes = await _context.GameTypes.ToListAsync();
        foreach (var gameTypeId in viewModel.SelectedGameTypeIds)
        {
            newParentGame.Games.Add(new Game
            {
                GameTypeId = gameTypeId,
                Title = gameTypes.Where(x => x.GameTypeId == gameTypeId).Select(x => x.Description).FirstOrDefault(),
                Description = "Δεν υπάρχουν πληροφορίες"
            });
        }

        _mediaService.CreateFolder($"games/{slug}");

        _context.ParentGames.Add(newParentGame);
        await _context.SaveChangesAsync();
    }

    public async Task<ParentGameEditViewModel> PrepareParentGameEditViewModel(Guid parentGameId, ClaimsPrincipal user)
    {
        await CanEditParentGame(parentGameId, user);

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Games)
            .ThenInclude(x => x.GameType)
            .FirstOrDefaultAsync(x => x.ParentGameId == parentGameId);

        if (parentGameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        var allGameTypes = await _context.GameTypes.ToListAsync();

        return new ParentGameEditViewModel
        {
            ParentGameId = parentGameDetails.ParentGameId,
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description,
            ProfileImageUrl = parentGameDetails.ProfileImagePath,
            SelectedGameTypeIds = parentGameDetails.Games.Select(g => g.GameTypeId).ToList(),
            GameTypes = allGameTypes
        };
    }

    public async Task<GameEditViewModel> PrepareGameEditViewModel(Guid gameId, ClaimsPrincipal user)
    {
        await CanEditGame(gameId, user);

        var gameDetails = await _context.Games.FirstOrDefaultAsync(x => x.GameId == gameId);

        if (gameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        return gameDetails.ToGameEditViewModel();
    }

    public async Task EditParentGame(ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user)
    {
        await CanEditParentGame(viewModel.ParentGameId, user);

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Games)
            .ThenInclude(g => g.PuzzleList)
            .ThenInclude(p => p.MediaFiles)
            .FirstOrDefaultAsync(x => x.ParentGameId == viewModel.ParentGameId);

        if (parentGameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        parentGameDetails.SubTitle = viewModel.SubTitle;
        parentGameDetails.Description = viewModel.Description;

        if (profileImage != null)
        {
            var profileImagePath = await _mediaService.SaveMediaFile(profileImage, $"games/{parentGameDetails.Slug}");
            parentGameDetails.ProfileImagePath = profileImagePath;
        }
        
        var currentGameTypeIds = parentGameDetails.Games.Select(g => g.GameTypeId).ToList();
        var gameTypesToAdd = viewModel.SelectedGameTypeIds.Except(currentGameTypeIds).ToList();
        var gameTypesToRemove = currentGameTypeIds.Except(viewModel.SelectedGameTypeIds).ToList();
        
        var gamesToRemove = parentGameDetails.Games.Where(g => gameTypesToRemove.Contains(g.GameTypeId)).ToList();
        foreach (var game in gamesToRemove)
        {
            foreach (var puzzle in game.PuzzleList)
            {
                foreach (var media in puzzle.MediaFiles)
                {
                    await DeletePuzzleMedia(puzzle.PuzzleId, media.MediaFileId, user);
                }

                _context.Puzzles.Remove(puzzle);
            }

            parentGameDetails.Games.Remove(game);
        }
        
        var gameTypes = await _context.GameTypes.ToListAsync();
        foreach (var gameTypeId in gameTypesToAdd)
        {
            parentGameDetails.Games.Add(new Game
            {
                GameTypeId = gameTypeId,
                Title = gameTypes.FirstOrDefault(x => x.GameTypeId == gameTypeId)?.Description ?? "Unknown",
                Description = "Δεν υπάρχουν πληροφορίες"
            });
        }

        _context.ParentGames.Update(parentGameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task EditGame(GameEditViewModel viewModel, ClaimsPrincipal user)
    {
        await CanEditGame(viewModel.GameId, user);

        var gameDetails = await _context.Games.FirstOrDefaultAsync(x => x.GameId == viewModel.GameId);

        if (gameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        gameDetails.Title = viewModel.Title;
        gameDetails.Description = viewModel.Description;

        _context.Games.Update(gameDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<ParentGameMediaViewModel> GetParentGameMedia(Guid parentGameId, ClaimsPrincipal user)
    {
        await CanEditParentGame(parentGameId, user);

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.ParentGameId == parentGameId);

        if (parentGameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        var viewModel = parentGameDetails.ToParentGameMediaViewModel();
        return viewModel;
    }

    public async Task AddParentGameMedia(Guid parentGameId, List<IFormFile> files, ClaimsPrincipal user)
    {
        await CanEditParentGame(parentGameId, user);

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.ParentGameId == parentGameId);

        if (parentGameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        foreach (var file in files)
        {
            var filePath = await _mediaService.SaveMediaFile(file, $"games/{parentGameDetails.Year}");
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

    public async Task DeleteParentGameMedia(Guid parentGameId, Guid mediafileId, ClaimsPrincipal user)
    {
        await CanEditParentGame(parentGameId, user);
        await _mediaService.DeleteMediaFile(mediafileId);
    }

    public async Task<PuzzleCreateViewModel> PrepareCreatePuzzleViewModel(Guid gameId, ClaimsPrincipal user)
    {
        await CanEditGame(gameId, user);

        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .Include(x => x.GameType)
            .Include(x => x.PuzzleList)
            .FirstOrDefaultAsync(x => x.GameId == gameId);

        if (gameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        var viewModel = gameDetails.ToGamePuzzleCreateViewModel();
        return viewModel;
    }

    public async Task CreatePuzzle(PuzzleCreateViewModel viewModel, ClaimsPrincipal user)
    {
        await CanEditGame(viewModel.GameId, user);

        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .Include(x => x.PuzzleList)
            .FirstOrDefaultAsync(x => viewModel.GameId == x.GameId);

        if (gameDetails == null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        var maxOrder = gameDetails.PuzzleList.Max(x => (short?)x.Order) ?? 0;

        var puzzle = new Puzzle
        {
            Game = gameDetails,
            Question = viewModel.Question,
            Answer = viewModel.Answer,
            Order = (short)(maxOrder + 1),
            Group = viewModel.Group,
            MediaFiles = new List<PuzzleMedia>()
        };

        if (viewModel.QuestionMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.QuestionMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{gameDetails.ParentGame.Year}");
                puzzle.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = "Q",
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
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{gameDetails.ParentGame.Year}");
                puzzle.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = "A",
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

    public async Task<PuzzleEditViewModel> PrepareEditPuzzleViewModel(Guid puzzleId, ClaimsPrincipal user)
    {
        await CanEditPuzzle(puzzleId, user);

        var puzzleDetails = await _context.Puzzles
            .Include(x => x.Game.GameType)
            .Include(x => x.Game.ParentGame)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);

        var viewModel = puzzleDetails.ToGamePuzzleEditViewModel();
        return viewModel;
    }

    public async Task<Guid> EditPuzzle(PuzzleEditViewModel viewModel, ClaimsPrincipal user)
    {
        await CanEditPuzzle(viewModel.PuzzleId, user);

        var puzzleDetails = await _context.Puzzles
            .Include(x => x.Game)
            .ThenInclude(x => x.ParentGame)
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.PuzzleId == viewModel.PuzzleId);

        puzzleDetails.Question = viewModel.Question;
        puzzleDetails.Answer = viewModel.Answer;
        puzzleDetails.Group = viewModel.Group is > 0 ? viewModel.Group.Value : null;

        if (viewModel.QuestionMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in viewModel.QuestionMediaFiles)
            {
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{puzzleDetails.Game.ParentGame.Year}");
                puzzleDetails.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = "Q",
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
                var mediaFilePath = await _mediaService.SaveMediaFile(mediaFile, $"games/{puzzleDetails.Game.ParentGame.Year}");
                puzzleDetails.MediaFiles.Add(new PuzzleMedia
                {
                    MediaType = "A",
                    MediaFile = new MediaFile
                    {
                        Path = mediaFilePath
                    }
                });
            }
        }

        _context.Puzzles.Update(puzzleDetails);
        await _context.SaveChangesAsync();
        return puzzleDetails.GameId;
    }

    public async Task<PuzzleDetailsViewModel> GetPuzzleMedia(Guid puzzleId, ClaimsPrincipal user)
    {
        await CanEditPuzzle(puzzleId, user);

        var puzzle = await _context.Puzzles
            .Include(x => x.MediaFiles)
            .ThenInclude(x => x.MediaFile)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);

        return puzzle.ToPuzzleDetailsViewModel();
    }

    public async Task<Guid> DeletePuzzle(Guid puzzleId, ClaimsPrincipal user)
    {
        await CanEditPuzzle(puzzleId, user);

        var puzzleToDelete = await _context.Puzzles
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);

        if (puzzleToDelete == null)
        {
            throw new NotFoundException("Ο γρίφος δεν βρέθηκε.");
        }

        var gameId = puzzleToDelete.GameId;
        var orderToDelete = puzzleToDelete.Order;
        var groupToDelete = puzzleToDelete.Group;

        foreach (var media in puzzleToDelete.MediaFiles)
        {
            await _mediaService.DeleteMediaFile(media.MediaFileId);
        }

        _context.Puzzles.Remove(puzzleToDelete);

        var remainingPuzzles = await _context.Puzzles
            .Where(p => p.GameId == gameId && p.Order > orderToDelete)
            .ToListAsync();

        foreach (var puzzle in remainingPuzzles)
        {
            puzzle.Order--;
        }

        if (groupToDelete.HasValue)
        {
            var puzzlesInSameGroup = await _context.Puzzles
                .Where(p => p.GameId == gameId && p.Group == groupToDelete && p.PuzzleId != puzzleId)
                .ToListAsync();

            if (puzzlesInSameGroup.Count == 1)
            {
                puzzlesInSameGroup[0].Group = null;
            }
        }

        await _context.SaveChangesAsync();
        return puzzleToDelete.GameId;
    }

    public async Task DeletePuzzleMedia(Guid puzzleId, Guid puzzleMediaId, ClaimsPrincipal user)
    {
        await CanEditPuzzle(puzzleId, user);

        var puzzleDetails = await _context.Puzzles
            .Include(x => x.MediaFiles)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);

        var mediaDetails = puzzleDetails.MediaFiles.FirstOrDefault(x => x.MediaFileId == puzzleMediaId);
        if (mediaDetails != null)
        {
            await _mediaService.DeleteMediaFile(mediaDetails.MediaFileId);
        }
    }

    private async Task CanEditParentGame(Guid parentGameId, ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(parentGameId);
        ArgumentNullException.ThrowIfNull(user);

        var parentGameDetails = await _context.ParentGames
            .Include(x => x.Host)
            .ThenInclude(x => x.Managers)
            .FirstOrDefaultAsync(x => x.ParentGameId == parentGameId);

        if (parentGameDetails is null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (parentGameDetails.Host.Managers.All(x => x.AppUserId != userId))
            {
                throw new UnauthorizedOperationException("Δεν έχετε δικαίωμα επεξεργασίας.");
            }
        }
    }

    private async Task CanEditGame(Guid gameId, ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(gameId);
        ArgumentNullException.ThrowIfNull(user);

        var gameDetails = await _context.Games
            .Include(x => x.ParentGame)
            .ThenInclude(x => x.Host)
            .ThenInclude(x => x.Managers)
            .FirstOrDefaultAsync(x => x.GameId == gameId);

        if (gameDetails is null)
        {
            throw new NotFoundException("Το παιχνίδι δεν βρέθηκε");
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (gameDetails.ParentGame.Host.Managers.All(x => x.AppUserId != userId))
            {
                throw new UnauthorizedOperationException("Δεν έχετε δικαίωμα επεξεργασίας.");
            }
        }
    }

    private async Task CanEditPuzzle(Guid puzzleId, ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(puzzleId);
        ArgumentNullException.ThrowIfNull(user);

        var puzzleDetails = await _context.Puzzles
            .Include(x => x.Game)
            .ThenInclude(x => x.ParentGame)
            .ThenInclude(x => x.Host)
            .ThenInclude(x => x.Managers)
            .FirstOrDefaultAsync(x => x.PuzzleId == puzzleId);

        if (puzzleDetails is null)
        {
            throw new NotFoundException("Ο γρίφος δεν βρέθηκε");
        }

        if (user.IsInRole("Manager") && !user.IsInRole("Admin"))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (puzzleDetails.Game.ParentGame.Host.Managers.All(x => x.AppUserId != userId))
            {
                throw new UnauthorizedOperationException("Δεν έχετε δικαίωμα επεξεργασίας του γρίφου.");
            }
        }
    }
}