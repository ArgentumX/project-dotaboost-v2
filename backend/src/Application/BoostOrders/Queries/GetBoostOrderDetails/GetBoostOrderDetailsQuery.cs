using Application.Common.Commands;
using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Queries.GetBoostOrderDetails;

public class GetBoostOrderDetailsQuery : SenderRequiredRequest<AuditableEntityDto>
{
    public Guid Id { get; init; }
}

public class GetBoostOrderDetailsHandler : IRequestHandler<GetBoostOrderDetailsQuery, AuditableEntityDto>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IApplicationDbContext _context;

    public GetBoostOrderDetailsHandler(IMapper mapper, IUserContext userContext, IApplicationDbContext context)
    {
        _mapper = mapper;
        _userContext = userContext;
        _context = context;
    }

    public async Task<AuditableEntityDto> Handle(GetBoostOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BoostOrders
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
            throw new NotFoundException(nameof(BoostOrder), request.Id);
        
        
        var isAdmin = _userContext.IsInRole(Roles.Administrator);
        var isOwner = request.SenderId == entity.UserId;
        var isBooster = entity.Booster != null && entity.Booster.UserId == request.SenderId;

        var hasFullAccess = isAdmin || isOwner || isBooster;
        
        return hasFullAccess
            ? _mapper.Map<BoostOrderDto>(entity)
            : _mapper.Map<PublicBoostOrderDto>(entity);
    }
}