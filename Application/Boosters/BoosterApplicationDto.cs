using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;

namespace Application.Boosters;

public class BoosterApplicationDto
{
    public Guid UserId { get; init; }
    public string Motivation { get; init; } = null!;
    public ApplicationStatus Status { get; init; }
    public string Contact { get; init; } = null!;
    public string SteamAccountLink { get; init; } = null!;
    public string? ReviewComment { get; init; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BoosterApplication, BoosterApplicationDto>();
            CreateMap<BoosterApplicationDto, BoosterApplication>();
        }
    }
}