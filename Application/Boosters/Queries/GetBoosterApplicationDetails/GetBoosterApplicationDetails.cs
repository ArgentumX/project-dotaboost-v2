using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Queries.GetBoosterApplicationDetails;

public class GetBoosterApplicationDetailsQuery : IRequest<BoosterApplicationDto>
{
    public Guid Id { get; init; }
}

public class GetBoosterApplicationDetailsHandler : IRequestHandler<GetBoosterApplicationDetailsQuery, BoosterApplicationDto>
{
    
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetBoosterApplicationDetailsHandler(IMapper mapper, IApplicationDbContext context, IUserContext userContext)
    {
        _mapper = mapper;
        _context = context;
        _userContext = userContext;
    }
    
    public async Task<BoosterApplicationDto> Handle(GetBoosterApplicationDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        
        var query = _context.BoosterApplications.AsQueryable();
        if (!_userContext.IsInRole("Admin")) 
            query = query.Where(application => application.UserId == userId);
        
        var entity = await query
            .Where(application => application.Id == request.Id)
            .ProjectTo<BoosterApplicationDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(BoosterApplication), request.Id);
        
        return entity;
    }
}