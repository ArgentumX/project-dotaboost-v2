using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Common.Behaviors;

public class InjectActorBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUserContext _userContext;

    public InjectActorBehavior(IUserContext userContext) =>
        _userContext = userContext;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IRequireActor { ActorId: null } actorAware)
        {
            actorAware.ActorId = _userContext.UserId;
        }

        return await next(cancellationToken);
    }
}