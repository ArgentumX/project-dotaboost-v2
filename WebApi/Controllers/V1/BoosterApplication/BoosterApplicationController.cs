using Application.Boosters;
using Application.Boosters.Commands.CreateBoosterApplication;
using Application.Boosters.Queries;
using Application.Boosters.Queries.GetBoosterApplicationDetails;
using Application.Boosters.Queries.GetBoosterApplications;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi.Controllers.V1.BoosterApplication;

[ApiVersion(1.0)]
public class BoosterApplicationController : BaseController
{
    
    
    [HttpGet]
    public async Task<Ok<PaginatedList<BoosterApplicationDto>>> GetBoosterApplications([FromQuery] BoosterApplicationFilter filter)
    {
        var query = new GetBoosterApplicationsQuery(filter);
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<Ok<BoosterApplicationDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBoosterApplicationDetailsQuery() {
            Id = id
        };
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }
    
    [HttpPost]
    public async Task<Ok<BoosterApplicationDto>> CreateBoostOrder([FromBody] CreateBoosterApplicationCommand command)
    {
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }
    
}