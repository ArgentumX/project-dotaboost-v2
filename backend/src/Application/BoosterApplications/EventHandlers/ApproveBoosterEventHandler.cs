using Application.Boosters;
using Application.Boosters.Commands.CreateBooster;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.BoosterApplications.EventHandlers;

public class ApproveBoosterEventHandler : INotificationHandler<ApproveBoosterEvent>
{
    private readonly IMediator _mediator;

    public ApproveBoosterEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(ApproveBoosterEvent notification, CancellationToken cancellationToken)
    {
        var newBooster = await CreateBooster(notification);
    }

    private async Task<BoosterDto> CreateBooster(ApproveBoosterEvent notification)
    {
        var command = new CreateBoosterCommand
        {
            SenderId = notification.Application.UserId
        };
        var result = await _mediator.Send(command);
        return result;
    }
}