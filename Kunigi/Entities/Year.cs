namespace Kunigi.Entities;

public class Year
{
    public int YearId { get; set; }

    public short Value { get; set; }
    
    public int HostId { get; set; }
    public virtual Team Host { get; set; }
    
    public int WinnerId { get; set; }
    public virtual Team Winner { get; set; }
}