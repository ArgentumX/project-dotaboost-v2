using Domain.Common;

namespace Domain.Entities;

public class Booster : BaseAuditableEntity
{
    public int UserId { get; set; }
    
    public int? OrderId { get; set; }
    public BoostOrder? Order { get; set; }

    public ICollection<Batch> Batches { get; }
}