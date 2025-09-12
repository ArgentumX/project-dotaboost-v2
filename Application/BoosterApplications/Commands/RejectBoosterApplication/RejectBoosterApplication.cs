using Application.BoosterApplications;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Boosters.Commands.RejectBoosterApplication;

public class RejectBoosterApplicationCommand : IRequest<BoosterApplicationDto>
{
    public Guid? ApplicationId { get; set; }
}


public class RejectBoosterApplicationValidator : AbstractValidator<RejectBoosterApplicationCommand>
{
    public RejectBoosterApplicationValidator()
    {
    }
}


public class RejectBoosterApplicationHandler : IRequestHandler<RejectBoosterApplicationCommand, BoosterApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RejectBoosterApplicationHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BoosterApplicationDto> Handle(RejectBoosterApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BoosterApplications.FirstOrDefaultAsync(application =>
                application.Id == request.ApplicationId,
            cancellationToken
        );
        
        if (entity == null)
            throw new NotFoundException(nameof(BoosterApplication), request.ApplicationId ?? Guid.Empty);

        if (entity.Status != ApplicationStatus.Pending)
            throw new BadRequestException("Application status is wrong");
        
        entity.Status = ApplicationStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<BoosterApplicationDto>(entity);
        return result;
    }
}