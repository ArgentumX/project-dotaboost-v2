using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

public class Booster : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public Guid? OrderId { get; set; }
    public BoostOrder? Order { get; set; }

    public ICollection<Batch> Batches { get; }


    public void TakeOrder(Guid orderId)
    {
        if (OrderId != null)
            throw new DomainException("Already taken other order!");

        OrderId = orderId;
    }

    public void RefuseOrder()
    {
        if (OrderId == null)
            throw new DomainException("No active order!");

        OrderId = null;
    }
}