using AutoMapper;
using Domain.Entities;

namespace Application.BoostOrders.Queries.GetBoostOrderDetails;

public class BoostOrderDto
{
    public Guid Id { get; init; }
    public string? Description { get; init; }
    public bool IsParty { get; init; }
    public bool IsPriority { get; init; }
    public string? SteamUsername { get; init; }
    public string? SteamPassword { get; init; }
    public bool IsPaid { get; init; }
    public bool IsClosed { get; init; }
    public int StartRating { get; init; }
    public int CurrentRating { get; init; }
    public int RequiredRating { get; init; }
    public Guid UserId { get; init; }
    public Guid? BoosterId { get; init; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BoostOrder, BoostOrderDto>();
        }
    }
}