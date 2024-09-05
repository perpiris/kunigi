using Kunigi.Entities;
using Kunigi.ViewModels;
using Kunigi.ViewModels.Game;

namespace Kunigi.Mappings;

public static class GameMappings
{
    public static ParentGameDetailsViewModel ToBaseParentGameDetailsViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameDetailsViewModel
        {
            Year = parentGameDetails.Year,
            Order = parentGameDetails.Order,
            Title = parentGameDetails.Title,
            Description = parentGameDetails.Description,
            Slug = parentGameDetails.Slug,
            ProfileImageUrl = parentGameDetails.ParentGameProfileImagePath
        };

        return viewModel;
    }

    public static ParentGameDetailsViewModel ToFullParentGameDetailsViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = parentGameDetails.ToBaseParentGameDetailsViewModel();

        viewModel.Winner = parentGameDetails.Winner.Name;
        viewModel.WinnerSlug = parentGameDetails.Winner.Slug;
        viewModel.Host = parentGameDetails.Host.Name;
        viewModel.HostSlug = parentGameDetails.Host.Slug;

        viewModel.GameList = [];

        foreach (var gameDetails in parentGameDetails.Games)
        {
            viewModel.GameList.Add(new GameDetailsViewModel
            {
                Id = gameDetails.GameId,
                Type = gameDetails.GameType.Description,
                Year = parentGameDetails.Year,
                Slug = gameDetails.GameType.Slug
            });
        }

        return viewModel;
    }
    
    public static GameDetailsViewModel ToBaseGameDetailsViewModel(this Game parentGameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            Id = parentGameDetails.GameId,
            Title = parentGameDetails.ParentGame.Title,
            Description = parentGameDetails.Description,
            Year = parentGameDetails.ParentGame.Year,
            Type = parentGameDetails.GameType.Description
        };

        return viewModel;
    }

    public static GameMediaViewModel ToGameMediaViewModel(short gameYear, List<ParentGameMedia> parentGameMedia)
    {
        var viewModel = new GameMediaViewModel
        {
            Year = gameYear,
            MediaFiles = parentGameMedia.Select(x => new MediaFileViewModel
            {
                Id = x.MediaFile.MediaFileId,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}