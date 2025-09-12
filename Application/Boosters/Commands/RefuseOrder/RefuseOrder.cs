using Application.Common.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Commands.RefuseOrder;

public class RefuseOrderCommand : SenderRequiredRequest<BoosterDto>
{
}

public class RefuseOrderHandler : IRequestHandler<RefuseOrderCommand, BoosterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RefuseOrderHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BoosterDto> Handle(RefuseOrderCommand request, CancellationToken cancellationToken)
    {
        var booster = await _context.Boosters.FirstOrDefaultAsync(x => x.UserId == request.SenderId, cancellationToken);
        if (booster == null)
            throw new BadRequestException("Not a booster!");

        booster.RefuseOrder();
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoosterDto>(booster);
        return result;
    }
}