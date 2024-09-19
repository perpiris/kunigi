using Kunigi.Entities;
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
            ParentGameId = parentGameDetails.ParentGameId,
            Year = parentGameDetails.Year,
            Order = parentGameDetails.Order,
            MainTitle = parentGameDetails.MainTitle,
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description,
            Slug = parentGameDetails.Slug,
            ProfileImageUrl = parentGameDetails.ProfileImagePath
        };

        if (includeFullDetails)
        {
            viewModel.Winner = parentGameDetails.Winner.Name;
            viewModel.WinnerId = parentGameDetails.Winner.TeamId;
            viewModel.Host = parentGameDetails.Host.Name;
            viewModel.HostId = parentGameDetails.Host.TeamId;

            viewModel.GameList = parentGameDetails.Games?
                .Select(gameDetails => gameDetails.ToGameDetailsViewModel())
                .ToList() ?? [];

            viewModel.MediaFiles = parentGameDetails.MediaFiles?
                .Select(teamMedia => new MediaFileViewModel
                {
                    MediaFileId = teamMedia.MediaFile.MediaFileId,
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
            GameId = gameDetails.GameId,
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
                GameId = p.GameId,
                PuzzleId = p.PuzzleId,
                Order = p.Order,
                Question = p.Question,
                Answer = p.Answer,
                QuestionMedia = p.MediaFiles?.Where(m => m.MediaType == "Q")
                    .Select(m => new MediaFileViewModel
                    {
                        MediaFileId = m.MediaFile.MediaFileId,
                        FileName = Path.GetFileName(m.MediaFile.Path),
                        Path = m.MediaFile.Path
                    }).ToList() ?? [],
                AnswerMedia = p.MediaFiles?.Where(m => m.MediaType == "A")
                    .Select(m => new MediaFileViewModel
                    {
                        MediaFileId = m.MediaFile.MediaFileId,
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
        var groupedPuzzleIds = new HashSet<Guid>();

        foreach (var puzzle in puzzles.OrderBy(p => p.Order))
        {
            if (groupedPuzzleIds.Contains(puzzle.PuzzleId))
                continue;

            if (!puzzle.Group.HasValue)
            {
                orderedPuzzles.Add(puzzle);
            }
            else
            {
                var group = groupedPuzzles.First(g => g.GroupName == puzzle.Group.ToString());
                orderedPuzzles.AddRange(group.Puzzles);
                groupedPuzzleIds.UnionWith(group.Puzzles.Select(p => p.PuzzleId));
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
            ParentGameId = parentGameDetails.ParentGameId,
            SubTitle = parentGameDetails.SubTitle,
            Description = parentGameDetails.Description
        };

        return viewModel;
    }

    public static GameEditViewModel ToGameEditViewModel(this Game gameDetails)
    {
        var viewModel = new GameEditViewModel
        {
            GameId = gameDetails.GameId,
            Description = gameDetails.Description
        };

        return viewModel;
    }

    public static PuzzleDetailsViewModel ToPuzzleDetailsViewModel(this Puzzle puzzleDetails)
    {
        var viewModel = new PuzzleDetailsViewModel
        {
            GameId = puzzleDetails.GameId,
            PuzzleId = puzzleDetails.PuzzleId,
            Order = puzzleDetails.Order,
            Question = puzzleDetails.Question,
            Answer = puzzleDetails.Answer,
            QuestionMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == "Q")
                .Select(m => new MediaFileViewModel
                {
                    MediaFileId = m.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(m.MediaFile.Path),
                    Path = m.MediaFile.Path
                }).ToList() ?? [],
            AnswerMedia = puzzleDetails.MediaFiles?.Where(m => m.MediaType == "A")
                .Select(m => new MediaFileViewModel
                {
                    MediaFileId = m.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(m.MediaFile.Path),
                    Path = m.MediaFile.Path
                }).ToList() ?? [],
            Group = puzzleDetails.Group
        };

        return viewModel;
    }

    public static PuzzleCreateViewModel ToGamePuzzleCreateViewModel(this Game gameDetails)
    {
        var viewModel = new PuzzleCreateViewModel
        {
            GameId = gameDetails.GameId
        };

        return viewModel;
    }

    public static PuzzleEditViewModel ToGamePuzzleEditViewModel(this Puzzle puzzleDetails)
    {
        var viewModel = new PuzzleEditViewModel
        {
            PuzzleId = puzzleDetails.PuzzleId,
            Group = puzzleDetails.Group,
            Question = puzzleDetails.Question,
            Answer = puzzleDetails.Answer
        };

        return viewModel;
    }

    public static ParentGameMediaViewModel ToParentGameMediaViewModel(this ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameMediaViewModel
        {
            ParentGameId = parentGameDetails.ParentGameId,
            YearSlug = parentGameDetails.Year.ToString(),
            Ttitle = parentGameDetails.MainTitle,
            MediaFiles = parentGameDetails.MediaFiles.Select(x => new MediaFileViewModel
            {
                MediaFileId = x.MediaFile.MediaFileId,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}