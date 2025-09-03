using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class BoostOrderController : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BoostOrderDto>> GetBoostOrderDetails(int id)
    {
        var query = new GetBoostOrderDetailsQuery 
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}