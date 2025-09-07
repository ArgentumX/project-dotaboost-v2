using Application.BoostOrders.Commands;
using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

// [Authorize]
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
    public async Task<ActionResult<int>> CreateBoostOrder(CreateBoostOrderCommand command)
    {
        command.UserId = UserId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}