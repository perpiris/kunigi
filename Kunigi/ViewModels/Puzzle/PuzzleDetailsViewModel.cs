namespace Kunigi.ViewModels.Puzzle;

public class PuzzleDetailsViewModel
{
    public int Id { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    public string Type { get; set; }
    
    public int Order { get; set; }
    
    public List<string> QuestionMedia { get; set; }
    
    public List<string> AnswerMedia { get; set; }
}