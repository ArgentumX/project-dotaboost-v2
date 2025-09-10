using AutoFilterer.Types;

namespace Application.Batches.Queries;

public class BatchFilter : PaginationFilterBase
{
    public int? ReceivedMmr { get; set; }
    public bool? IsWin { get; set; }
    public Guid? BoostOrderId { get; set; }
    public Guid? BoosterId { get; set; }
}