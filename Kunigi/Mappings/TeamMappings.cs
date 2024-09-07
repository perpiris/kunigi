using Kunigi.Entities;
using Kunigi.ViewModels;
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
            Slug = teamDetails.Slug,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.TeamProfileImagePath,
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
                    Title = year.Title,
                    Year = year.Year
                })
                .ToList() ?? [];

            viewModel.GamesHosted = teamDetails.HostedGames?
                .Select(year => new ParentGameDetailsViewModel
                {
                    Title = year.Title,
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
            Slug = teamDetails.Slug,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.TeamProfileImagePath,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website
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