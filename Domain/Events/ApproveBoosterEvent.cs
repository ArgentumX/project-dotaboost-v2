using System.Net.Mime;
using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class ApproveBoosterEvent : BaseEvent
{
    public BoosterApplication Application { get; }

    public ApproveBoosterEvent(BoosterApplication application)
    {
        Application = application;
    }
}