using Kunigi.ViewModels.Common;

namespace Kunigi.ViewModels.Puzzle;

public class PuzzleDetailsViewModel
{
    public int Id { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    public string Type { get; set; }
    
    public int Order { get; set; }
    
    public List<MediaFileViewModel> QuestionMedia { get; set; }
    
    public List<MediaFileViewModel> AnswerMedia { get; set; }
}