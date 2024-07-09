namespace Kunigi.Entities;

public class Game
{
    public int Id { get; set; }

    public string Category { get; set; }

    public int YearId { get; set; }
    public virtual Year Year { get; set; }
}