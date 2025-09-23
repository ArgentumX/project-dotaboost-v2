using Application.Common.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.BoostOrders;

public class PublicBoostOrderDto : AuditableEntityDto
{
    public bool IsParty { get; init; }
    public bool IsPriority { get; init; }
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
            CreateMap<BoostOrder, PublicBoostOrderDto>();
        }
    }
}