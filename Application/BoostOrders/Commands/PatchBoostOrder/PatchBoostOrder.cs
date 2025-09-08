namespace Application.BoostOrders.Commands.PatchBoostOrder;

using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class PatchBoostOrderCommand : IRequest<int>
{
    public string Description { get; set; } = "";
    public int? Id { get; set; }
    public Guid? UserId { get; set; }
}

public class PatchBoostOrderCommandValidator : AbstractValidator<PatchBoostOrderCommand>
{
    public PatchBoostOrderCommandValidator() {
        RuleFor(c => c.Description)
            .MaximumLength(256);
    }
}



public class PatchBoostOrderHandler : IRequestHandler<PatchBoostOrderCommand, int>
{
    private readonly IApplicationDbContext _context;

    public PatchBoostOrderHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(PatchBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BoostOrders.FirstOrDefaultAsync(order =>
            order.Id == request.Id, cancellationToken);
        
        if (entity == null || entity.UserId != request.UserId)
            throw new NotFoundException(nameof(BoostOrder), request.Id);
        
        entity.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}