using Domain.Common;

namespace Domain.Entities;

public class BoostOrder : BaseAuditableEntity
{
    public string? Description { get; set; }
}