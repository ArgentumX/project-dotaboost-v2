using AutoFilterer.Types;
using Domain.Common.Enum;

namespace Application.Boosters.Queries;

public class BoosterFilter : PaginationFilterBase
{
    public Guid? UserId { get; set; }
    public ApplicationStatus? Status { get; set; }
}