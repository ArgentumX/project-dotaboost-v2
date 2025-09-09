using Domain.Common;
using Domain.Common.Enum;

namespace Domain.Entities;

public class BoosterApplication : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Motivation { get; set; } = String.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string Contact { get; set; } = String.Empty;
    public string SteamAccountLink { get; set; } = String.Empty;
    public string? ReviewComment { get; set; }
}