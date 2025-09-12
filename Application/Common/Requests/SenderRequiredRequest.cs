using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Commands;

public abstract class SenderRequiredRequest<TResponse> : IRequest<TResponse>, IRequireSender
{
    [JsonIgnore] public Guid? SenderId { get; set; }
}