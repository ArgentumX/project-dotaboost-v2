using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Queries.GetBoosterDetails;

public class GetBoosterDetailsQuery : IRequest<BoosterDto>
{
    public Guid Id { get; init; }
}

public class GetBoosterDetailsHandler : IRequestHandler<GetBoosterDetailsQuery, BoosterDto>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public GetBoosterDetailsHandler(IMapper mapper, IApplicationDbContext context, IUserContext userContext)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<BoosterDto> Handle(GetBoosterDetailsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.BoosterApplications.AsQueryable();
        var entity = await query
            .Where(application => application.Id == request.Id)
            .ProjectTo<BoosterDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
            throw new NotFoundException(nameof(BoosterApplication), request.Id);

        return entity;
    }
}