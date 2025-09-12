using Application.Common.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Boosters;

public class BoosterDto : AuditableEntityDto
{
    public Guid UserId { get; init; }
    public Guid? OrderId { get; init; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Booster, BoosterDto>();
        }
    }
}