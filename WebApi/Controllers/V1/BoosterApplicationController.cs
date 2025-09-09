using Application.Boosters;
using Application.Boosters.Commands.CreateBoosterApplication;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(1.0)]
public class BoosterApplicationController : BaseController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoosterApplicationDto>> GetBoostOrderDetails(Guid id)
    {
        return Ok();
    }


    [HttpPost]
    public async Task<ActionResult<BoosterApplicationDto>> CreateBoostOrder([FromBody] CreateBoosterApplicationCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}