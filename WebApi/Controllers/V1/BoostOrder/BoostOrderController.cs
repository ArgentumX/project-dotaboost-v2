using Application.BoostOrders.Commands;
using Application.BoostOrders.Commands.DeleteBoostOrder;
using Application.BoostOrders.Commands.PatchBoostOrder;
using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

// [Authorize]
[ApiVersion(1.0)]
public class BoostOrderController : BaseController
{
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BoostOrderDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBoostOrderDetailsQuery
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<BoostOrderDto>> CreateBoostOrder([FromBody] CreateBoostOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/close")]
    public async Task<ActionResult<BoostOrderDto>> CloseBoostOrder(Guid id)
    {
        var command = new CloseBoostOrderCommand
        {
            Id = id
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BoostOrderDto>> PatchBoostOrder(Guid id, [FromBody] PatchBoostOrderCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}