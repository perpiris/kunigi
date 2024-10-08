﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class PuzzleMedia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid PuzzleMediaId { get; set; }
    
    public Guid PuzzleId { get; set; }
    
    public Guid MediaFileId { get; set; }
    
    [MaxLength(1)]
    public string MediaType { get; set; }
    
    public virtual Puzzle Puzzle { get; set; }
    
    public virtual MediaFile MediaFile { get; set; }
}