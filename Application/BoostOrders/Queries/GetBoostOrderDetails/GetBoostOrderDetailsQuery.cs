using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Queries.GetBoostOrderDetails;

public class GetBoostOrderDetailsQuery : IRequest<BoostOrderDto>
{
    public int Id { get; set; }
}

public class GetBoostOrderDetailsHandler : IRequestHandler<GetBoostOrderDetailsQuery, BoostOrderDto>
{
    
    private readonly IMapper _mapper;
    private readonly IBoostOrderDbContext _context;

    public GetBoostOrderDetailsHandler(IMapper mapper, IBoostOrderDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    
    public async Task<BoostOrderDto> Handle(GetBoostOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BoostOrders
            .Where(x => x.Id == request.Id)
            .ProjectTo<BoostOrderDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
        {
            throw new NotFoundException(nameof(BoostOrder), request.Id);
        }
        return entity;
    }
}