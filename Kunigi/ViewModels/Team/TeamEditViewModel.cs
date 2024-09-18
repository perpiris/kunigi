using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kunigi.ViewModels.Team;

public class TeamEditViewModel
{
    public Guid TeamId { get; set; }
    
    [DisplayName("Όνομα Ομάδας")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [DisplayName("Έτος Δημιουργίας")]
    [Range(1900, 2100, ErrorMessage = "Παρακαλώ εισάγετε έγκυρο έτος.")]
    public short? CreatedYear { get; set; }

    [DisplayName("Ενεργή")]
    public bool IsActive { get; set; }
    
    [DisplayName("Περιγραφή")]
    public string Description { get; set; }
    
    [StringLength(255)]
    public string Website { get; set; }
    
    [StringLength(255)]
    public string Facebook { get; set; }
    
    [StringLength(255)]
    public string Youtube { get; set; }
    
    [StringLength(255)]
    public string Instagram { get; set; }
    
    [DisplayName("Φωτογραφία")]
    public string ProfileImageUrl { get; set; }
}