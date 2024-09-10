using Kunigi.ViewModels.Common;

namespace Kunigi.ViewModels.Game;

public class PuzzleDetailsViewModel
{
    public int Id { get; set; }
    
    public string Question { get; set; }
    
    public string Answer { get; set; }
    
    public string Type { get; set; }
    
    public short Order { get; set; }

    public short Group { get; set; }
    
    public List<MediaFileViewModel> QuestionMedia { get; set; }
    
    public List<MediaFileViewModel> AnswerMedia { get; set; }
}