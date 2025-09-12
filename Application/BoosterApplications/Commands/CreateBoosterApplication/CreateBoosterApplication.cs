using Application.Common.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Common.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BoosterApplications.Commands.CreateBoosterApplication;

public class CreateBoosterApplicationCommand : ActorCommand<BoosterApplicationDto>
{
    public string Motivation { get; set; } = String.Empty;
    public string Contact { get; set; } = String.Empty;
    public string SteamAccountLink { get; set; } = String.Empty;
}

public class CreateBoosterApplicationValidator : AbstractValidator<CreateBoosterApplicationCommand>
{
    public CreateBoosterApplicationValidator()
    {
        // RuleFor(x => x.Motivation)
        //     .NotEmpty().WithMessage("Укажи мотивацию, почему хочешь стать бустером")
        //     .MinimumLength(20).WithMessage("Мотивация должна содержать минимум 20 символов");
        //
        // RuleFor(x => x.Contact)
        //     .NotEmpty().WithMessage("Укажи контакт для связи (Discord, Telegram, Email)")
        //     .MaximumLength(100).WithMessage("Контакт слишком длинный");
        //
        // RuleFor(x => x.SteamAccountLink)
        //     .NotEmpty().WithMessage("Укажи ссылку на Steam аккаунт")
        //     .Must(link => Uri.TryCreate(link, UriKind.Absolute, out var uri) 
        //                   && (uri.Host.Contains("steamcommunity.com") || uri.Host.Contains("steampowered.com")))
        //     .WithMessage("Укажи корректную ссылку на Steam профиль");
    }
}


public class CreateBoosterApplicationHandler : IRequestHandler<CreateBoosterApplicationCommand, BoosterApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBoosterApplicationHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BoosterApplicationDto> Handle(CreateBoosterApplicationCommand request, CancellationToken cancellationToken)
    {
       
        
        bool hasActiveApplications = await _context.BoosterApplications.AnyAsync(application =>
                application.UserId == request.ActorId &&
                application.Status == ApplicationStatus.Pending,
            cancellationToken
        );
        
        if (hasActiveApplications)
            throw new BadRequestException("You cannot create new application with other active applications");
        
        var entity = new BoosterApplication
        {
            Motivation = request.Motivation,    
            Contact = request.Contact,
            SteamAccountLink = request.SteamAccountLink,
            UserId = (Guid)request.ActorId!
        };
        
        await _context.BoosterApplications.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var result = _mapper.Map<BoosterApplicationDto>(entity);
        return result;
    }
}