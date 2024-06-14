using AutoMapper;
using Kunigi.Entities;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;

namespace Kunigi.Utilities;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Team, TeamDetailsViewModel>();
        CreateMap<Team, TeamEditViewModel>();
        
        CreateMap<Game, GameDetailsViewModel>()
            .ForMember(x => x.Host, y => y.MapFrom(z => z.Host.Name))
            .ForMember(x => x.Winner, y => y.MapFrom(z => z.Winner.Name));
        CreateMap<Game, GameEditViewModel>();
    }
}