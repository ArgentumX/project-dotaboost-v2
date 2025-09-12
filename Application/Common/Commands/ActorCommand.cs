using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Commands;

public abstract class ActorCommand<TResponse> : IRequest<TResponse>, IRequireActor
{
    [JsonIgnore]
    public Guid? ActorId { get; set; }

}