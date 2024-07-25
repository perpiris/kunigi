namespace Kunigi.ViewModels.GameYear;

public class GameYearDetailsViewModel
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    
    public short Year { get; set; }

    public string GetFullTitle()
    {
        return $"{Title} ({Year})";
    }
}