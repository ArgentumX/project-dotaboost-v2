using Application.BoostOrders;
using Application.BoostOrders.Commands;
using Application.BoostOrders.Commands.DeleteBoostOrder;
using Application.BoostOrders.Commands.PatchBoostOrder;
using Application.BoostOrders.Queries;
using Application.BoostOrders.Queries.GetBoosterOrders;
using Application.BoostOrders.Queries.GetBoostOrderDetails;
using Application.Common.Dtos;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi.Controllers;

[Authorize]
[ApiVersion(1.0)]
public class BoostOrderController : BaseController
{
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<Ok<AuditableEntityDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBoostOrderDetailsQuery
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<Ok<PaginatedList<PublicBoostOrderDto>>> GetBoostOrders([FromQuery] BoostOrderFilter filter)
    {
        var query = new GetBoosterOrdersQuery(filter);
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    [HttpPost]
    public async Task<Ok<BoostOrderDto>> CreateBoostOrder([FromBody] CreateBoostOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }

    [HttpPost("{id:guid}/close")]
    public async Task<Ok<BoostOrderDto>> CloseBoostOrder(Guid id)
    {
        var command = new CloseBoostOrderCommand
        {
            Id = id
        };
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<Ok<BoostOrderDto>> PatchBoostOrder(Guid id, [FromBody] PatchBoostOrderCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }
}