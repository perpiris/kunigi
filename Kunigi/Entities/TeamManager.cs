using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kunigi.Entities;

public class TeamManager
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid TeamManagerId { get; set; }

    public Guid TeamId { get; set; }

    public string AppUserId { get; set; }
    
    public virtual Team Team { get; set; }
    
    public virtual AppUser User { get; set; }
}