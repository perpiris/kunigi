using Kunigi.Entities;
using Kunigi.ViewModels.Common;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;

namespace Kunigi.Mappings;

public static class TeamMappings
{
    public static TeamDetailsViewModel ToTeamDetailsViewModel(this Team teamDetails, bool includeFullDetails = false)
    {
        var viewModel = new TeamDetailsViewModel
        {
            TeamId = teamDetails.TeamId,
            Name = teamDetails.Name,
            Description = teamDetails.Description,
            CreatedYear = teamDetails.CreatedYear,
            IsActive = teamDetails.IsActive,
            ProfileImagePath = teamDetails.ProfileImagePath,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website
        };

        if (includeFullDetails)
        {
            viewModel.GamesWon = teamDetails.WonGames?
                .Select(year => new ParentGameDetailsViewModel
                {
                    ParentGameId = year.ParentGameId,
                    MainTitle = year.MainTitle,
                    Year = year.Year
                })
                .ToList() ?? [];

            viewModel.GamesHosted = teamDetails.HostedGames?
                .Select(year => new ParentGameDetailsViewModel
                {
                    ParentGameId = year.ParentGameId,
                    MainTitle = year.MainTitle,
                    Year = year.Year
                })
                .ToList() ?? [];

            viewModel.MediaFiles = teamDetails.MediaFiles?
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

    public static TeamEditViewModel ToTeamEditViewModel(this Team teamDetails)
    {
        var viewModel = new TeamEditViewModel
        {
            TeamId = teamDetails.TeamId,
            Name = teamDetails.Name,
            CreatedYear = teamDetails.CreatedYear,
            IsActive = teamDetails.IsActive,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImagePath,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website
        };

        return viewModel;
    }

    public static TeamManagerEditViewModel ToTeamManagerEditViewModel(this Team teamDetails)
    {
        var viewModel = new TeamManagerEditViewModel
        {
            TeamId = teamDetails.TeamId,
            ManagerList = teamDetails.Managers.Select(x => new TeamManagerDetailsViewModel
            {
                AppUserId = x.AppUserId,
                Email = x.User.Email
            }).ToList()
        };

        return viewModel;
    }

    public static TeamMediaViewModel ToTeamMediaViewModel(this Team teamDetails)
    {
        var viewModel = new TeamMediaViewModel
        {
            TeamId = teamDetails.TeamId,
            Name = teamDetails.Name,
            TeamSlug = teamDetails.Slug,
            MediaFiles = teamDetails.MediaFiles.Select(x => new MediaFileViewModel
            {
                MediaFileId = x.MediaFile.MediaFileId,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}