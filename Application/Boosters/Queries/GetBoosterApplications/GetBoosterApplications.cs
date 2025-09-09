using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Queries.GetBoosterApplications;

public class GetBoosterApplicationsQuery : IRequest<PaginatedList<BoosterApplicationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetBoosterApplicationsHandler 
    : IRequestHandler<GetBoosterApplicationsQuery, PaginatedList<BoosterApplicationDto>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetBoosterApplicationsHandler(
        IMapper mapper, 
        IApplicationDbContext context, 
        IUserContext userContext)
    {
        _mapper = mapper;
        _context = context;
        _userContext = userContext;
    }

    public async Task<PaginatedList<BoosterApplicationDto>> Handle(
        GetBoosterApplicationsQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _context.BoosterApplications.AsQueryable();
        if (!_userContext.IsInRole("Admin"))
            query = query.Where(app => app.UserId == _userContext.UserId);
        
        var result = await query
            .ProjectTo<BoosterApplicationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return result;
    }
}