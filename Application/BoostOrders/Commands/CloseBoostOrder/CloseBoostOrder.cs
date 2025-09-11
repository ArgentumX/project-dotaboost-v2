using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Commands.DeleteBoostOrder;

public class CloseBoostOrderCommand : IRequest<BoostOrderDto>
{
    public Guid? Id { get; set; }
}

public class CloseBoostOrderCommandValidator : AbstractValidator<CloseBoostOrderCommand>
{
    public CloseBoostOrderCommandValidator() {
    }
}



public class CloseBoostOrderHandler : IRequestHandler<CloseBoostOrderCommand, BoostOrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public CloseBoostOrderHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IMapper mapper
        )
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
    }
    public async Task<BoostOrderDto> Handle(CloseBoostOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == null)
            throw new BadRequestException("Id cannot be null");
        
        var userId = _userContext.UserId;

        var entity = await _context.BoostOrders.FirstOrDefaultAsync(order =>
            order.Id == request.Id && order.IsClosed == false, cancellationToken);
        
        if (entity == null || entity.UserId != userId)
            throw new NotFoundException(nameof(BoostOrder), request.Id);

        if (entity.Booster != null)
            throw new BadRequestException("It is not possible to close orders with active boosters");
        
        entity.IsClosed = true;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<BoostOrderDto>(entity);
        return result;
    }
}