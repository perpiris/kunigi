namespace Kunigi.ViewModels.Puzzle;

public class PuzzleDetailsViewModel
{
    public int Id { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    public int QuestionMediaFilesCount { get; set; }
    
    public int AnswerMediaFilesCount { get; set; }
    
    public string Type { get; set; }
    
    public int Order { get; set; }
}