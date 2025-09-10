using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.Batches.Queries.GetBatches;

public class GetBatchsQuery(BatchFilter queryFilter) : IRequest<PaginatedList<BatchDto>>
{
    public BatchFilter QueryFilter { get; init; } = queryFilter;
}

public class GetBatchsHandler 
    : IRequestHandler<GetBatchsQuery, PaginatedList<BatchDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetBatchsHandler(
        IMapper mapper, 
        IApplicationDbContext context, 
        IUserContext userContext)
    {
        _mapper = mapper;
        _context = context;
        _userContext = userContext;
    }

    public async Task<PaginatedList<BatchDto>> Handle(
        GetBatchsQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _context.Batches.AsQueryable();
        query = query.ApplyFilterWithoutPagination(request.QueryFilter);
        
        var result = await query
            .ProjectTo<BatchDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.QueryFilter.Page, request.QueryFilter.PerPage, cancellationToken);

        return result;
    }
}