using System.Security.Claims;
using Kunigi.ViewModels.Game;

namespace Kunigi.Services;

public interface IGameService
{
    Task<List<ParentGameDetailsViewModel>> GetAllGames();
    
    Task<ParentGameDetailsViewModel> GetParentGameDetails(short gameYear);
    
    Task<GameDetailsViewModel> GetGameDetails(short gameYear, string gameTypeSlug);

    Task<ParentGameCreateViewModel> PrepareCreateParentGameViewModel(ParentGameCreateViewModel viewModel);

    Task CreateParentGame(ParentGameCreateViewModel viewModel);
    
    Task<ParentGameEditViewModel> PrepareEditParentGameViewModel(short gameYear, ClaimsPrincipal user);

    Task EditParentGame(short gameYear, ParentGameEditViewModel viewModel, IFormFile profileImage, ClaimsPrincipal user);
    
    Task<GameMediaViewModel> GetTeamMedia(short gameYear, ClaimsPrincipal user);

    Task AddGameMedia(short gameYear, List<IFormFile> files, ClaimsPrincipal user);

    Task DeleteGameMedia(short gameYear, int mediafileId, ClaimsPrincipal user);
}