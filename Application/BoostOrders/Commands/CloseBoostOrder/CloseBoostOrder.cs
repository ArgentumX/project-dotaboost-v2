using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Commands.DeleteBoostOrder;

public class CloseBoostOrderCommand : IRequest<int>
{
    public int? Id { get; set; }
    public Guid? UserId { get; set; }
}

public class CloseBoostOrderCommandValidator : AbstractValidator<CloseBoostOrderCommand>
{
    public CloseBoostOrderCommandValidator() {
    }
}



public class CloseBoostOrderHandler : IRequestHandler<CloseBoostOrderCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CloseBoostOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CloseBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

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