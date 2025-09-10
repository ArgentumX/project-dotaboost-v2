using Application.Boosters;
using Application.Boosters.Commands.CreateBoosterApplication;
using Application.Boosters.Queries;
using Application.Boosters.Queries.GetBoosterApplicationDetails;
using Application.Boosters.Queries.GetBoosterApplications;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.BoosterApplication;

[ApiVersion(1.0)]
public class BoosterApplicationController : BaseController
{
    
    
    [HttpGet]
    public async Task<ActionResult<PaginatedList<BoosterApplicationDto>>> GetBoosterApplications([FromQuery] BoosterApplicationFilter filter)
    {
        var query = new GetBoosterApplicationsQuery(filter);
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoosterApplicationDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBoosterApplicationDetailsQuery() {
            Id = id
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<BoosterApplicationDto>> CreateBoostOrder([FromBody] CreateBoosterApplicationCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
}