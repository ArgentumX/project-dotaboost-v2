using Domain.Common;
using Domain.Common.Enum;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public class BoosterApplication : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Motivation { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string Contact { get; set; } = string.Empty;
    public string SteamAccountLink { get; set; } = string.Empty;
    public string? ReviewComment { get; set; }


    public void Approve()
    {
        if (Status != ApplicationStatus.Pending)
            throw new DomainException("Application must be in Pending status");
        Status = ApplicationStatus.Approved;
        AddDomainEvent(new ApproveBoosterEvent(this));
    }

    public void Reject()
    {
        if (Status != ApplicationStatus.Pending)
            throw new DomainException("Application must be in Pending status");
        Status = ApplicationStatus.Rejected;
        AddDomainEvent(new RejectBoosterEvent(this));
    }
}