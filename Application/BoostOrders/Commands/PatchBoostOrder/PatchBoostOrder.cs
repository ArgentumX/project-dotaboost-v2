namespace Application.BoostOrders.Commands.PatchBoostOrder;

using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class PatchBoostOrderCommand : IRequest<Guid>
{
    public string Description { get; set; } = "";
    public Guid? Id { get; set; }
}

public class PatchBoostOrderCommandValidator : AbstractValidator<PatchBoostOrderCommand>
{
    public PatchBoostOrderCommandValidator() {
        RuleFor(c => c.Description)
            .MaximumLength(256);
    }
}



public class PatchBoostOrderHandler : IRequestHandler<PatchBoostOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PatchBoostOrderHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(PatchBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var entity = await _context.BoostOrders.FirstOrDefaultAsync(order =>
            order.Id == request.Id, cancellationToken);
        
        if (entity == null || entity.UserId != userId)
            throw new NotFoundException(nameof(BoostOrder), request.Id);
        
        entity.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}