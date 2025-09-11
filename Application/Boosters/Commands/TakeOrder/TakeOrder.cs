using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Commands.TakeOrder;

public class TakeOrderCommand : IRequest<BoosterDto>
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
}

public class TakeOrderHandler : IRequestHandler<TakeOrderCommand, BoosterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TakeOrderHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BoosterDto> Handle(TakeOrderCommand request, CancellationToken cancellationToken)
    { 
        var booster = await _context.Boosters.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
        if (booster == null)
            throw new BadRequestException("Not a booster!");
        
        booster.TakeOrder(request.OrderId);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoosterDto>(booster);
        return result;
    }
}