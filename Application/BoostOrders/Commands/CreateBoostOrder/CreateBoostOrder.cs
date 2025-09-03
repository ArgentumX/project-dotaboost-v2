using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.BoostOrders.Commands;

public class CreateBoostOrderCommand : IRequest<int>
{
    public int Id { get; set; }
    public string? Description { get; set; }
}


public class CreateBoostOrderHandler : IRequestHandler<CreateBoostOrderCommand, int>
{
    private readonly IBoostOrderDbContext _context;

    public CreateBoostOrderHandler(IBoostOrderDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateBoostOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = new BoostOrder
        {
            Description = request.Description,
        };
        await _context.BoostOrders.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}