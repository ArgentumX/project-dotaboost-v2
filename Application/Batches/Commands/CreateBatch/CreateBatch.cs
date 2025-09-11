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
    public Guid BoostOrderId { get; set; }
}

public class CreateBatchValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchValidator()
    {
        RuleFor(x => x.BoostOrderId)
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
            x.OrderId != null && x.UserId == userId && x.OrderId == request.BoostOrderId);
        
        if (booster == null)
            throw new NotFoundException(nameof(BoostOrder), request.BoostOrderId);
        
        
        var entity = new Batch
        {
            Screen = request.Screen,
            ReceivedMmr = request.ReceivedMmr,
            IsWin = request.IsWin,
            BoostOrderId = request.BoostOrderId,
            BoosterId = booster.Id
        };
        
        await _context.Batches.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BatchDto>(entity);
        return result;
    }
}