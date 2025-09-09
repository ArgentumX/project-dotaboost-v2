using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Commands.DeleteBoostOrder;

public class CloseBoostOrderCommand : IRequest<Guid>
{
    public Guid? Id { get; set; }
}

public class CloseBoostOrderCommandValidator : AbstractValidator<CloseBoostOrderCommand>
{
    public CloseBoostOrderCommandValidator() {
    }
}



public class CloseBoostOrderHandler : IRequestHandler<CloseBoostOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CloseBoostOrderHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    public async Task<Guid> Handle(CloseBoostOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == null)
            throw new BadRequestException("Id cannot be null");
        
        var userId = _currentUserService.UserId;

        var entity = await _context.BoostOrders.FirstOrDefaultAsync(order =>
            order.Id == request.Id && order.IsClosed == false, cancellationToken);
        
        if (entity == null || entity.UserId != userId)
            throw new NotFoundException(nameof(BoostOrder), request.Id);

        if (entity.Booster != null)
            throw new BadRequestException("It is not possible to close orders with active boosters");
        
        entity.IsClosed = true;
        
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}