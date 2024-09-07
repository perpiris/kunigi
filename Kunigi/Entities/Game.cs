﻿namespace Kunigi.Entities;

public class Game
{
    public int GameId { get; set; }
    
    public string Description { get; set; }

    public int GameTypeId { get; set; }
    
    public int ParentGameId { get; set; }
    
    public virtual GameType GameType { get; set; }
    
    public virtual ParentGame ParentGame { get; set; }
    
    public virtual ICollection<Puzzle> PuzzleList { get; set; }
}