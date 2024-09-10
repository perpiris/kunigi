using System.Security.Claims;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;

namespace Kunigi.Services;

public interface IGameService
{
    Task<PaginatedViewModel<ParentGameDetailsViewModel>> GetPaginatedGame(int pageNumber = 1, int pageSize = 10);

    Task<ParentGameDetailsViewModel> GetParentGameDetails(short gameYear, ClaimsPrincipal user = null);
    
    Task<GameDetailsViewModel> GetGameDetails(short gameYear, string gameTypeSlug, ClaimsPrincipal user = null);
    
    Task<GamePuzzleDetailsViewModel> GetGamePuzzleList(short gameYear, string gameTypeSlug);

    Task<ParentGameCreateViewModel> PrepareCreateParentGameViewModel(ParentGameCreateViewModel viewModel);

    Task CreateParentGame(ParentGameCreateViewModel viewModel);
    
    Task<ParentGameEditViewModel> PrepareParentGameEditViewModel(short gameYear, ClaimsPrincipal user);
    
    Task<GameEditViewModel> PrepareGameEditViewModel(short gameYear, string gameTypeSlug, ClaimsPrincipal user);

    Task EditParentGame(short gameYear, ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task EditGame(short gameYear, string gameTypeSlug, GameEditViewModel viewModel, ClaimsPrincipal user);
    
    Task<ParentGameMediaViewModel> GetParentGameMEdia(short gameYear, ClaimsPrincipal user);

    Task AddParentGameMedia(short gameYear, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteParentGameMedia(short gameYear, int mediafileId, ClaimsPrincipal user);

    Task<GamePuzzleCreateViewModel> PrepareCreateGamePuzzleViewModel(short gameYear, string gameTypeSlug);

    Task CreateGamePuzzle(GamePuzzleCreateViewModel viewModel);
}