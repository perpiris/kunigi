using Kunigi.Entities;
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

            viewModel.MediaFiles = parentGameDetails.MediaFiles?
                .Select(teamMedia => new MediaFileViewModel
                {
                    Id = teamMedia.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(teamMedia.MediaFile.Path),
                    Path = teamMedia.MediaFile.Path
                })
                .ToList() ?? [];
        }

        return viewModel;
    }

    public static GameDetailsViewModel ToGameDetailsViewModel(this Game gameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            ParentGameTitle = gameDetails.ParentGame.MainTitle,
            GameTitle = gameDetails.Title,
            Description = gameDetails.Description,
            Year = gameDetails.ParentGame.Year,
            GameType = gameDetails.GameType.Description,
            TypeSlug = gameDetails.GameType.Slug
        };

        return viewModel;
    }

    public static GamePuzzleDetailsViewModel ToGamePuzzleDetailsViewModel(this Game gameDetails)
    {
        var puzzles = gameDetails.PuzzleList
            .Select(p => new PuzzleDetailsViewModel
            {
                Id = p.PuzzleId,
                Order = p.Order,
                Question = p.Question,
                Answer = p.Answer,
                QuestionMedia = p.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Question)
                    .Select(m => new MediaFileViewModel
                    {
                        Id = m.MediaFile.MediaFileId,
                        FileName = Path.GetFileName(m.MediaFile.Path),
                        Path = m.MediaFile.Path
                    }).ToList() ?? [],
                AnswerMedia = p.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Answer)
                    .Select(m => new MediaFileViewModel
                    {
                        Id = m.MediaFile.MediaFileId,
                        FileName = Path.GetFileName(m.MediaFile.Path),
                        Path = m.MediaFile.Path
                    }).ToList() ?? [],
                Group = p.Group
            })
            .ToList();

        var groupedPuzzles = puzzles
            .GroupBy(p => p.Group)
            .Select(g => new GamePuzzleGroupViewModel
            {
                GroupName = g.Key.ToString(),
                Puzzles = g.OrderBy(p => p.Order).ToList(),
                MinOrder = g.Min(p => p.Order)
            })
            .OrderBy(g => g.MinOrder)
            .ToList();

        var orderedPuzzles = new List<PuzzleDetailsViewModel>();
        var groupedPuzzleIds = new HashSet<int>();

        foreach (var puzzle in puzzles.OrderBy(p => p.Order))
        {
            if (groupedPuzzleIds.Contains(puzzle.Id))
                continue;

            if (!puzzle.Group.HasValue)
            {
                orderedPuzzles.Add(puzzle);
            }
            else
            {
                var group = groupedPuzzles.First(g => g.GroupName == puzzle.Group.ToString());
                orderedPuzzles.AddRange(group.Puzzles);
                groupedPuzzleIds.UnionWith(group.Puzzles.Select(p => p.Id));
            }
        }

        var viewModel = new GamePuzzleDetailsViewModel
        {
            GameDetails = gameDetails.ToGameDetailsViewModel(),
            PuzzleList = orderedPuzzles,
            GroupedPuzzles = groupedPuzzles
        };

        return viewModel;
    }

    public static ParentGameEditViewModel ToParentGameEditViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameEditViewModel
        {
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description,
            GameYear = parentGameDetails.Year
        };

        return viewModel;
    }

    public static GameEditViewModel ToGameEditViewModel(this Game gameDetails)
    {
        var viewModel = new GameEditViewModel
        {
            Description = gameDetails.Description,
            GameType = gameDetails.GameType.Description,
            GameTypeSlug = gameDetails.GameType?.Slug,
            GameYear = gameDetails.ParentGame.Year
        };

        return viewModel;
    }
    
    public static PuzzleDetailsViewModel ToPuzzleDetailsViewModel(this Puzzle puzzleDetails)
    {
        var viewModel = new PuzzleDetailsViewModel
        {
            Id = puzzleDetails.PuzzleId,
            Order = puzzleDetails.Order,
            Question = puzzleDetails.Question,
            Answer = puzzleDetails.Answer,
            QuestionMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Question)
                .Select(m => new MediaFileViewModel
                {
                    Id = m.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(m.MediaFile.Path),
                    Path = m.MediaFile.Path
                }).ToList() ?? [],
            AnswerMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == PuzzleMediaType.Answer)
                .Select(m => new MediaFileViewModel
                {
                    Id = m.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(m.MediaFile.Path),
                    Path = m.MediaFile.Path
                }).ToList() ?? [],
            Group = puzzleDetails.Group
        };

        return viewModel;
    }

    public static GamePuzzleCreateViewModel ToGamePuzzleCreateViewModel(this Game gameDetails)
    {
        var viewModel = new GamePuzzleCreateViewModel
        {
            GameId = gameDetails.GameId,
            GameTypeSlug = gameDetails.GameType.Slug,
            GameYear = gameDetails.ParentGame.Year
        };

        return viewModel;
    }

    public static GamePuzzleEditViewModel ToGamePuzzleEditViewModel(this Puzzle puzzleDetails)
    {
        var viewModel = new GamePuzzleEditViewModel
        {
            PuzzleId = puzzleDetails.PuzzleId,
            Group = puzzleDetails.Group,
            Question = puzzleDetails.Question,
            Answer = puzzleDetails.Answer,
            GameTypeSlug = puzzleDetails.Game.GameType.Slug,
            GameYear = puzzleDetails.Game.ParentGame.Year
        };

        return viewModel;
    }

    public static ParentGameMediaViewModel ToParentGameMediaViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameMediaViewModel
        {
            YearSlug = parentGameDetails.Year.ToString(),
            Ttitle = parentGameDetails.MainTitle,
            MediaFiles = parentGameDetails.MediaFiles.Select(x => new MediaFileViewModel
            {
                Id = x.MediaFile.MediaFileId,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}