using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Boosters.Commands.ApproveBoosterApplication;

public class ApproveBoosterApplicationCommand : IRequest<BoosterApplicationDto>
{
    public Guid? ApplicationId { get; set; }
}


public class ApproveBoosterApplicationValidator : AbstractValidator<ApproveBoosterApplicationCommand>
{
    public ApproveBoosterApplicationValidator()
    {
    }
}


public class ApproveBoosterApplicationHandler : IRequestHandler<ApproveBoosterApplicationCommand, BoosterApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public ApproveBoosterApplicationHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        IMapper mapper
    )
    {
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
    }
    public async Task<BoosterApplicationDto> Handle(ApproveBoosterApplicationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BoosterApplications.FirstOrDefaultAsync(application =>
                application.Id == request.ApplicationId,
            cancellationToken
        );
        
        if (entity == null)
            throw new NotFoundException(nameof(BoosterApplication), request.ApplicationId ?? Guid.Empty);
        
        if (entity.Status != ApplicationStatus.Pending)
            throw new BadRequestException("Application status is wrong");

        
        entity.Status = ApplicationStatus.Approved;
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<BoosterApplicationDto>(entity);
        return result;
    }
}