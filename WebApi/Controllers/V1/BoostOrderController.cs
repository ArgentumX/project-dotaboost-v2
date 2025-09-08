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
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<BoostOrderDto>> GetBoostOrderDetails(int id)
    {
        var query = new GetBoostOrderDetailsQuery
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<int>> CreateBoostOrder([FromBody] CreateBoostOrderCommand command)
    {
        command.SetUserId(UserId);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:int}/close")]
    public async Task<ActionResult> CloseBoostOrder(int id, [FromBody] CloseBoostOrderCommand command)
    {
        command.UserId = UserId;
        command.Id = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> PatchBoostOrder(int id, [FromBody] PatchBoostOrderCommand command)
    {
        command.UserId = UserId;
        command.Id = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}