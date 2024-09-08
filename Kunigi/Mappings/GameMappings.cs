﻿using Kunigi.Entities;
using Kunigi.Enums;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;

namespace Kunigi.Mappings;

public static class GameMappings
{
    public static ParentGameDetailsViewModel ToParentGameDetailsViewModel(this ParentGame parentGameDetails,
        bool includeFullDetails = false)
    {
        var viewModel = new ParentGameDetailsViewModel
        {
            Year = parentGameDetails.Year,
            Order = parentGameDetails.Order,
            MainTitle = parentGameDetails.MainTitle,
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description,
            Slug = parentGameDetails.Slug,
            ProfileImageUrl = parentGameDetails.ParentGameProfileImagePath
        };

        if (includeFullDetails)
        {
            viewModel.Winner = parentGameDetails.Winner?.Name;
            viewModel.WinnerSlug = parentGameDetails.Winner?.Slug;
            viewModel.Host = parentGameDetails.Host?.Name;
            viewModel.HostSlug = parentGameDetails.Host?.Slug;

            viewModel.GameList = parentGameDetails.Games?
                .Select(gameDetails => gameDetails.ToGameDetailsViewModel())
                .ToList() ?? [];
        }

        return viewModel;
    }
    
    public static ParentGameEditViewModel ToParentGameEditViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameEditViewModel
        {
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description
        };

        return viewModel;
    }

    public static GameDetailsViewModel ToGameDetailsViewModel(this Game gameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            Title = gameDetails.ParentGame.MainTitle,
            Description = gameDetails.Description,
            Year = gameDetails.ParentGame.Year,
            GameType = gameDetails.GameType.Description,
            TypeSlug = gameDetails.GameType.Slug
        };

        return viewModel;
    }

    public static ParentGameMediaViewModel ToGameMediaViewModel(short gameYear, List<ParentGameMedia> parentGameMedia)
    {
        var viewModel = new ParentGameMediaViewModel
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

    public static GamePuzzleDetailsViewModel ToGamePuzzleDetailsViewModel(this Game gameDetails)
    {
        var viewModel = new GamePuzzleDetailsViewModel
        {
            GameDetails = gameDetails.ToGameDetailsViewModel(),
            PuzzleList = gameDetails.PuzzleList.Select(ToPuzzleDetailsViewModel).ToList()
        };

        viewModel.PuzzleList = gameDetails.PuzzleList.Select(ToPuzzleDetailsViewModel)
            .OrderBy(x => x.Order).ToList();

        return viewModel;
    }

    private static PuzzleDetailsViewModel ToPuzzleDetailsViewModel(this Puzzle puzzleDetails)
    {
        var viewModel = new PuzzleDetailsViewModel
        {
            Id = puzzleDetails.PuzzleId,
            Order = puzzleDetails.Order,
            Question = puzzleDetails.Question,
            Answer = puzzleDetails.Answer,
            QuestionMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Question).Select(m => new MediaFileViewModel
            {
                Id = m.MediaFile.MediaFileId,
                FileName = Path.GetFileName(m.MediaFile.Path),
                Path = m.MediaFile.Path
            }).ToList() ?? [],
            AnswerMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Answer).Select(m => new MediaFileViewModel
            {
                Id = m.MediaFile.MediaFileId,
                FileName = Path.GetFileName(m.MediaFile.Path),
                Path = m.MediaFile.Path
            }).ToList() ?? []
        };

        return viewModel;
    }
}