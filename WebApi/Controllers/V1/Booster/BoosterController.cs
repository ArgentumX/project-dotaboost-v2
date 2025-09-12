using Application.Boosters;
using Application.Boosters.Commands.RefuseOrder;
using Application.Boosters.Commands.TakeOrder;
using Application.Boosters.Queries;
using Application.Boosters.Queries.GetBoosterDetails;
using Application.Boosters.Queries.GetBoosters;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Booster;

[ApiVersion(1.0)]
public class BoosterController : BaseController
{
    [HttpPost("take")]
    public async Task<Ok<BoosterDto>> TakeOrder([FromBody] TakeOrderDto dto)
    {
        var command = new TakeOrderCommand
        {
            SenderId = UserId,
            OrderId = dto.OrderId
        };
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }

    [HttpPost("refuse")]
    public async Task<Ok<BoosterDto>> RefuseOrder()
    {
        var command = new RefuseOrderCommand { };
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }

    [HttpGet]
    public async Task<Ok<PaginatedList<BoosterDto>>> GetBatches([FromQuery] BoosterFilter filter)
    {
        var query = new GetBoostersQuery(filter);
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<Ok<BoosterDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBoosterDetailsQuery
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }
}

public record TakeOrderDto(Guid OrderId);