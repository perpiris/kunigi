using Kunigi.Entities;
using Kunigi.ViewModels;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;

namespace Kunigi.Mappings;

public static class TeamMappings
{
    public static TeamDetailsViewModel ToBaseInfoViewModel(this Team teamDetails)
    {
        var viewModel = new TeamDetailsViewModel
        {
            Name = teamDetails.Name,
            Slug = teamDetails.Slug,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImageUrl,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website,
        };

        return viewModel;
    }

    public static TeamDetailsViewModel ToFullInfoViewModel(this Team teamDetails)
    {
        var viewModel = teamDetails.ToBaseInfoViewModel();

        viewModel.GamesWon = [];
        viewModel.GamesHosted = [];
        viewModel.MediaFiles = [];

        if (teamDetails.WonYears != null)
        {
            foreach (var year in teamDetails.WonYears)
            {
                viewModel.GamesWon.Add(new ParentGameDetailsViewModel
                {
                    Id = year.Id,
                    Title = year.Title,
                    Year = year.Year
                });
            }
        }

        if (teamDetails.HostedYears != null)
        {
            foreach (var year in teamDetails.HostedYears)
            {
                viewModel.GamesHosted.Add(new ParentGameDetailsViewModel
                {
                    Id = year.Id,
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
                    Id = teamMedia.MediaFile.Id,
                    FileName = Path.GetFileName(teamMedia.MediaFile.Path),
                    Path = teamMedia.MediaFile.Path
                });
            }
        }

        return viewModel;
    }

    public static TeamEditViewModel ToEditViewModel(this Team teamDetails)
    {
        var viewModel = new TeamEditViewModel
        {
            Name = teamDetails.Name,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImageUrl,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website,
        };

        return viewModel;
    }

    public static TeamMediaViewModel ToMediaViewModel(string teamSlug, List<TeamMedia> teamMedia)
    {
        var viewModel = new TeamMediaViewModel
        {
            TeamSlug = teamSlug,
            MediaFiles = teamMedia.Select(x => new MediaFileViewModel
            {
                Id = x.MediaFile.Id,
                FileName = Path.GetFileName(x.MediaFile.Path),
                Path = x.MediaFile.Path
            }).ToList()
        };

        return viewModel;
    }
}