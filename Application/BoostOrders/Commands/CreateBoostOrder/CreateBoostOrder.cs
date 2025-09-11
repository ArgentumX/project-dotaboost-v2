using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoostOrders.Commands;

public class CreateBoostOrderCommand : IRequest<BoostOrderDto>
{
    public string? Description { get; set; }
    public bool IsParty { get; set; } = true;
    public bool IsPriority { get; set; } = false;
    public string? SteamUsername { get; set; }
    public string? SteamPassword { get; set; }
    public int StartRating { get; set; }
    public int RequiredRating { get; set; }
}

public class CreateBoostOrderCommandValidator : AbstractValidator<CreateBoostOrderCommand>
{
    public CreateBoostOrderCommandValidator()
    {
        RuleFor(c => c.Description)
            .MaximumLength(256);
        RuleFor(c => c.StartRating)
            .GreaterThanOrEqualTo(0);
        RuleFor(c => c.RequiredRating)
            .GreaterThan(c => c.StartRating);
    }
}


public class CreateBoostOrderHandler : IRequestHandler<CreateBoostOrderCommand, BoostOrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public CreateBoostOrderHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IMapper mapper
    )
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
    }
    public async Task<BoostOrderDto> Handle(CreateBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
       
        
        bool hasActiveOrder = await _context.BoostOrders.AnyAsync(o =>
                o.UserId == userId &&
                o.IsClosed == false,
            cancellationToken
        );
        
        if (hasActiveOrder)
            throw new BadRequestException("You cannot create new order with other active order");
        
        var entity = new BoostOrder
        {
            Description = request.Description,
            IsParty = request.IsParty,
            IsPriority = request.IsPriority,
            SteamUsername = request.SteamUsername,
            SteamPassword = request.SteamPassword,
            StartRating =  request.StartRating,
            CurrentRating =  request.StartRating,
            RequiredRating =  request.RequiredRating,
            UserId = userId, 
        };
        await _context.BoostOrders.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoostOrderDto>(entity);
        return result;
    }
}