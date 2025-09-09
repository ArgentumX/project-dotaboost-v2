using Domain.Common;

namespace Domain.Entities;

public class Batch : BaseAuditableEntity
{
    public string Screen { get; set; } = null!;
    public int ReceivedMmr { get; set; }
    public bool IsWin { get; set; }
    
    public int BoostOrderId { get; set; }
    public BoostOrder BoostOrder { get; set; } = null!;
    
    public Guid BoosterId { get; set; }
    public Booster Booster { get; set; } = null!;
}