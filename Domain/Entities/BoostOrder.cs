using Domain.Common;

namespace Domain.Entities;

public class BoostOrder : BaseAuditableEntity
{
    public string? Description { get; set; }
    public bool IsParty { get; set; } = true;
    public bool IsPriority { get; set; } = false;
    public string? SteamUsername { get; set; }
    public string? SteamPassword { get; set; }
    public bool IsPaid { get; set; } = false;
    public bool IsClosed { get; set; } = false;
    public int StartRating { get; set; }
    public int CurrentRating { get; set; }
    public int RequiredRating { get; set; }
    
    public int UserId  { get; set; }
    
    public Booster? Booster { get; set; }

    public List<Batch> Batches { get; }
}