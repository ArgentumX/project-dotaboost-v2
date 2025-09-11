using Application.Batches;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Queries.GetBatchDetails;

public class GetBatchDetailsQuery : IRequest<BatchDto>
{
    public Guid Id { get; init; }
}

public class GetBatchDetailsHandler : IRequestHandler<GetBatchDetailsQuery, BatchDto>
{
    
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetBatchDetailsHandler(IMapper mapper, IApplicationDbContext context, IUserContext userContext)
    {
        _mapper = mapper;
        _context = context;
        _userContext = userContext;
    }

    public async Task<BatchDto> Handle(GetBatchDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        var query = _context.Batches.AsQueryable();
        var entity = await query
            .Where(x => x.Id == request.Id)
            .ProjectTo<BatchDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(Batch), request.Id);
        
        return entity;
    }
}