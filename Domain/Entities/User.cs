using Domain.Common;

namespace Domain.Entities;

public class User : BaseAuditableEntity
{
    public List<BoostOrder> BoostOrders { get; }
    public Booster? Booster { get; set; }
}