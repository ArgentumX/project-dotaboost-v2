using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

public class RejectBoosterEvent : BaseEvent
{
    public BoosterApplication Application { get; }

    public RejectBoosterEvent(BoosterApplication application)
    {
        Application = application;
    }
}