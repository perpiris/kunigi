﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kunigi.ViewModels.GameYear;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kunigi.ViewModels.Team;

public class TeamDetailsViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string Description { get; set; }
    
    public string Website { get; set; }
    
    public string Facebook { get; set; }
    
    public string Youtube { get; set; }
    
    public string Instagram { get; set; }
    
    public string ProfileImageUrl { get; set; }
    
    public SelectList ManagerSelectList { get; set; }

    public List<GameYearDetailsViewModel> GamesWon { get; set; }
    
    public List<GameYearDetailsViewModel> GamesHosted { get; set; }
}