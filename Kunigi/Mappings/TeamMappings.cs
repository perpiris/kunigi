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
            Name = teamDetails.Name,
            TeamSlug = teamDetails.Slug,
            Description = teamDetails.Description,
            CreatedYear = teamDetails.CreatedYear,
            IsActive = teamDetails.IsActive,
            ProfileImageUrl = teamDetails.ProfileImagePath,
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
                    MainTitle = year.MainTitle,
                    Year = year.Year
                })
                .ToList() ?? [];

            viewModel.GamesHosted = teamDetails.HostedGames?
                .Select(year => new ParentGameDetailsViewModel
                {
                    MainTitle = year.MainTitle,
                    Year = year.Year
                })
                .ToList() ?? [];

            viewModel.MediaFiles = teamDetails.MediaFiles?
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

    public static TeamEditViewModel ToTeamEditViewModel(this Team teamDetails)
    {
        var viewModel = new TeamEditViewModel
        {
            Name = teamDetails.Name,
            TeamSlug = teamDetails.Slug,
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
            Name = teamDetails.Name,
            TeamSlug = teamDetails.Slug,
            ManagerList = teamDetails.Managers.Select(m => new TeamManagerDetailsViewModel
            {
                Id = m.Id,
                Email = m.Email
            }).ToList()
        };

        return viewModel;
    }

    public static TeamMediaViewModel ToTeamMediaViewModel(Team teamDetails, List<TeamMedia> teamMedia)
    {
        var viewModel = new TeamMediaViewModel
        {
            Name = teamDetails.Name,
            TeamSlug = teamDetails.Slug,
            MediaFiles = teamMedia.Select(x => new MediaFileViewModel
            {
                Id = x.MediaFile.MediaFileId,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}