using System.Security.Claims;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;

namespace Kunigi.Services;

public interface IGameService
{
    Task<PaginatedViewModel<ParentGameDetailsViewModel>> GetPaginatedParentGames(int pageNumber = 1, int pageSize = 10);

    Task<ParentGameDetailsViewModel> GetParentGameDetails(Guid parentGameId, ClaimsPrincipal user = null);
    
    Task<GameDetailsViewModel> GetGameDetails(Guid gameId, ClaimsPrincipal user = null);
    
    Task<GamePuzzleDetailsViewModel> GetGamePuzzleList(Guid gameId);

    Task<ParentGameCreateViewModel> PrepareCreateParentGameViewModel(ParentGameCreateViewModel viewModel);

    Task CreateParentGame(ParentGameCreateViewModel viewModel);
    
    Task<ParentGameEditViewModel> PrepareParentGameEditViewModel(Guid parentGameId, ClaimsPrincipal user);

    Task EditParentGame(ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task<GameEditViewModel> PrepareGameEditViewModel(Guid gameId, ClaimsPrincipal user);
    
    Task EditGame(GameEditViewModel viewModel, ClaimsPrincipal user);
    
    Task<ParentGameMediaViewModel> GetParentGameMedia(Guid parentGameId, ClaimsPrincipal user);

    Task AddParentGameMedia(Guid parentGameId, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteParentGameMedia(Guid parentGameId, Guid mediafileId, ClaimsPrincipal user);

    Task<PuzzleCreateViewModel> PrepareCreatePuzzleViewModel(Guid gameId, ClaimsPrincipal user);

    Task CreatePuzzle(PuzzleCreateViewModel viewModel, ClaimsPrincipal user);
    
    Task<PuzzleEditViewModel> PrepareEditPuzzleViewModel(Guid puzzleId, ClaimsPrincipal user);

    Task EditPuzzle(PuzzleEditViewModel viewModel, ClaimsPrincipal user);
    
    Task<PuzzleDetailsViewModel> GetPuzzleMedia(Guid puzzleId, ClaimsPrincipal user);
    
    Task<Guid> DeletePuzzle(Guid puzzleId, ClaimsPrincipal user);

    Task DeletePuzzleMedia(Guid puzzleId, Guid mediaId, ClaimsPrincipal user);
}