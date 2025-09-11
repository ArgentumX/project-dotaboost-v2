using Application.Boosters;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Batches.Commands.CreateBatch;

public class CreateBatchCommand : IRequest<BatchDto>
{
    public string Screen { get; set; } = null!;
    public int ReceivedMmr { get; set; }
    public bool IsWin { get; set; }
    public Guid OrderId { get; set; }
}

public class CreateBatchValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.IsWin)
            .NotEmpty();
        
        RuleFor(x => x.ReceivedMmr)
            .NotEmpty();
    }
}


public class CreateBatchHandler : IRequestHandler<CreateBatchCommand, BatchDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public CreateBatchHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IMapper mapper
    )
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
    }
    public async Task<BatchDto> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        
        var booster = await _context.Boosters.FirstOrDefaultAsync(x =>
            x.UserId == userId);
        
        if (booster == null)
            throw new NotFoundException(nameof(Booster), userId);
        
        if (booster.OrderId != request.OrderId)
            throw new BadRequestException("Order id does not match");
        
        
        var entity = new Batch
        {
            Screen = request.Screen,
            ReceivedMmr = request.ReceivedMmr,
            IsWin = request.IsWin,
            BoostOrderId = request.OrderId,
            BoosterId = booster.Id
        };
        
        await _context.Batches.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BatchDto>(entity);
        return result;
    }
}