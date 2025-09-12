using Application.Common.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Commands.CreateBooster;

public class CreateBoosterCommand : ActorCommand<BoosterDto>
{
}

public class CreateBoosterValidator : AbstractValidator<CreateBoosterCommand>
{
    public CreateBoosterValidator()
    {
        RuleFor(x => x.ActorId).NotEmpty();
    }
}


public class CreateBoosterHandler : IRequestHandler<CreateBoosterCommand, BoosterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBoosterHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BoosterDto> Handle(CreateBoosterCommand request, CancellationToken cancellationToken)
    {
        var isBooster = _context.Boosters.Any(x => x.UserId == request.ActorId);
        if (isBooster)
            throw new BadRequestException("Already booster");
          
        var entity = new Booster
        {
            UserId = (Guid)request.ActorId!,
        };
        
        await _context.Boosters.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoosterDto>(entity);
        return result;
    }
}