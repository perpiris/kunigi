﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class ParentGame
{
    public int Id { get; set; }

    public short Year { get; set; }

    public short Order { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string Slug { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ProfileImageUrl { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public string ParentGameFolderUrl { get; set; }
    
    public int HostId { get; set; }
    
    public int WinnerId { get; set; }
    
    [ForeignKey("HostId")]
    public virtual Team Host { get; set; }
    
    [ForeignKey("WinnerId")]
    public virtual Team Winner { get; set; }
    
    public virtual ICollection<Game> Games { get; set; }
    
    public virtual ICollection<ParentGameMedia> MediaFiles { get; set; }
}