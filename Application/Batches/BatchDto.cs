using Application.Common.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Batches;

public class BatchDto : AuditableEntityDto
{
    public string Screen { get; init; }  = null!;
    public int ReceivedMmr { get; init; }
    public bool IsWin { get; init; }
    public Guid OrderId { get; init; }
    public Guid BoosterId { get; init; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Batch, BatchDto>();
        }
    }
}