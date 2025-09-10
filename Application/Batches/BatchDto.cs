using AutoMapper;
using Domain.Entities;

namespace Application.Batches;

public class BatchDto
{
    public Guid Id { get; init; }
    public string Screen { get; init; }  = null!;
    public int ReceivedMmr { get; init; }
    public bool IsWin { get; init; }
    public Guid BoostOrderId { get; init; }
    public Guid BoosterId { get; init; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Batch, BatchDto>();
            CreateMap<BatchDto, Batch>();
        }
    }
}