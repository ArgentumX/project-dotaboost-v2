using Application.Boosters;
using Application.Boosters.Commands.ApproveBoosterApplication;
using Application.Boosters.Commands.RejectBoosterApplication;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.BoosterApplication;

[ApiVersion(1.0)]
public class AdminBoosterApplicationController : BaseController
{
    [HttpPost("{id:guid}/reject")]
    public async Task<Ok<BoosterApplicationDto>> RejectBoostApplication(Guid id)
    {
        var command = new RejectBoosterApplicationCommand
        {
            ApplicationId = id
        };
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }


    [HttpPost("{id:guid}/approve")]
    public async Task<Ok<BoosterApplicationDto>> ApproveBoostApplication(Guid id)
    {
        var command = new ApproveBoosterApplicationCommand
        {
            ApplicationId = id
        };
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }
}