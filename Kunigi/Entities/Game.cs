﻿namespace Kunigi.Entities;

public class Game
{
    public int Id { get; set; }
    
    public string Description { get; set; }

    public int GameTypeId { get; set; }
    
    public int GameYearId { get; set; }
    
    public virtual GameType GameType { get; set; }
    
    public virtual GameYear GameYear { get; set; }
}