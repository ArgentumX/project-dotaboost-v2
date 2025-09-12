using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Queries.GetBoosters;

public class GetBoostersQuery(BoosterFilter queryFilter) : IRequest<PaginatedList<BoosterDto>>
{
    public BoosterFilter QueryFilter { get; init; } = queryFilter;
}

public class GetBoostersHandler 
    : IRequestHandler<GetBoostersQuery, PaginatedList<BoosterDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetBoostersHandler(
        IMapper mapper, 
        IApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<PaginatedList<BoosterDto>> Handle(
        GetBoostersQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _context.Boosters.AsQueryable();
        query = query.ApplyFilterWithoutPagination(request.QueryFilter);
        var result = await query
            .ProjectTo<BoosterDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.QueryFilter.Page, request.QueryFilter.PerPage, cancellationToken);
        return result;
    }
}