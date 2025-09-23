using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;

namespace Application.BoostOrders.Queries.GetBoosterOrders;

public class GetBoosterOrdersQuery(BoostOrderFilter queryFilter) : IRequest<PaginatedList<PublicBoostOrderDto>>
{
    public BoostOrderFilter QueryFilter { get; init; } = queryFilter;
}

public class GetBoosterOrdersHandler
    : IRequestHandler<GetBoosterOrdersQuery, PaginatedList<PublicBoostOrderDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetBoosterOrdersHandler(
        IMapper mapper,
        IApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<PaginatedList<PublicBoostOrderDto>> Handle(
        GetBoosterOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.BoostOrders.AsQueryable();
        query = query.ApplyFilterWithoutPagination(request.QueryFilter);
        var result = await query
            .ProjectTo<PublicBoostOrderDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.QueryFilter.Page, request.QueryFilter.PerPage, cancellationToken);

        return result;
    }
}