using Application.BoostOrders.Queries.GetBoostOrderDetails;
using AutoMapper;

namespace Application.BoostOrders.Commands.PatchBoostOrder;

using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class PatchBoostOrderCommand : IRequest<BoostOrderDto>
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



public class PatchBoostOrderHandler : IRequestHandler<PatchBoostOrderCommand, BoostOrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public PatchBoostOrderHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IMapper mapper
    )
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<BoostOrderDto> Handle(PatchBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        var entity = await _context.BoostOrders.FirstOrDefaultAsync(order =>
            order.Id == request.Id, cancellationToken);
        
        if (entity == null || entity.UserId != userId)
            throw new NotFoundException(nameof(BoostOrder), request.Id);
        
        entity.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoostOrderDto>(entity);
        return result;
    }
}