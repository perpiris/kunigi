﻿namespace Kunigi.Entities;

public class Puzzle
{
    public int Id { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }

    public int GameId { get; set; }
}