using AutoFilterer.Types;

namespace Application.BoostOrders.Queries;

public class BoostOrderFilter : PaginationFilterBase
{
    public bool? IsParty { get; set; }
    public bool? IsPriority { get; set; }
    public bool? IsPaid { get; set; }
    public bool? IsClosed { get; set; }
    public int? StartRating { get; set; }
    public int? CurrentRating { get; set; }
    public int? RequiredRating { get; set; }
    public Guid? UserId { get; set; }
}