using Kunigi.Entities;
using Kunigi.ViewModels;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;

namespace Kunigi.Mappings;

public static class TeamMappings
{
    public static TeamDetailsViewModel ToBaseTeamDetailsViewModel(this Team teamDetails)
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

        return viewModel;
    }

    public static TeamDetailsViewModel ToFullTeamDetailsViewModel(this Team teamDetails)
    {
        var viewModel = teamDetails.ToBaseTeamDetailsViewModel();

        viewModel.GamesWon = [];
        viewModel.GamesHosted = [];
        viewModel.MediaFiles = [];

        if (teamDetails.WonGames != null)
        {
            foreach (var year in teamDetails.WonGames)
            {
                viewModel.GamesWon.Add(new ParentGameDetailsViewModel
                {
                    Title = year.Title,
                    Year = year.Year
                });
            }
        }

        if (teamDetails.HostedGames != null)
        {
            foreach (var year in teamDetails.HostedGames)
            {
                viewModel.GamesHosted.Add(new ParentGameDetailsViewModel
                {
                    Title = year.Title,
                    Year = year.Year
                });
            }
        }

        if (teamDetails.MediaFiles != null)
        {
            foreach (var teamMedia in teamDetails.MediaFiles)
            {
                viewModel.MediaFiles.Add(new MediaFileViewModel
                {
                    Id = teamMedia.MediaFile.MediaFileId,
                    FileName = Path.GetFileName(teamMedia.MediaFile.Path),
                    Path = teamMedia.MediaFile.Path
                });
            }
        }

        return viewModel;
    }

    public static TeamEditViewModel ToTeamEditViewModel(this Team teamDetails)
    {
        var viewModel = new TeamEditViewModel
        {
            Name = teamDetails.Name,
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