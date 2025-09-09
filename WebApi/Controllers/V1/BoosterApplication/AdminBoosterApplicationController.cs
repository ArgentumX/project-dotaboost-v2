using Application.Boosters;
using Application.Boosters.Commands.ApproveBoosterApplication;
using Application.Boosters.Commands.CreateBoosterApplication;
using Application.Boosters.Commands.RejectBoosterApplication;
using Application.Boosters.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.BoosterApplication;

[ApiVersion(1.0)]
public class AdminBoosterApplicationController : BaseController
{
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<BoosterApplicationDto>> RejectBoostApplication(Guid id)
    {
        var command = new RejectBoosterApplicationCommand
        {
            ApplicationId = id
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<BoosterApplicationDto>> ApproveBoostApplication(Guid id)
    {
        var command = new ApproveBoosterApplicationCommand
        {
            ApplicationId = id
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}