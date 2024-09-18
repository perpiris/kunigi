﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class Team
{
    public int TeamId { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Name { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    public short? CreatedYear { get; set; }
    
    public bool IsActive { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Website { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Facebook { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Youtube { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Instagram { get; set; }
    
    public string ProfileImagePath { get; set; }
    
    public virtual ICollection<ParentGame> HostedGames { get; set; }
    
    public virtual ICollection<ParentGame> WonGames { get; set; }
    
    public virtual ICollection<AppUser> Managers { get; set; }
    
    public virtual ICollection<TeamMedia> MediaFiles { get; set; }
}