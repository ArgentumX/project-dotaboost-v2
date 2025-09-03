using AutoMapper;
using Domain.Entities;

namespace Application.BoostOrders.Queries.GetBoostOrderDetails;

public class BoostOrderDto
{
    public int Id { get; init; }
    public string? Description { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BoostOrder, BoostOrderDto>();
        }
    }
}