using Application.Batches;
using Application.Batches.Commands.CreateBatch;
using Application.Batches.Queries;
using Application.Batches.Queries.GetBatches;
using Application.Boosters.Queries.GetBatchDetails;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Batches;

[ApiVersion(1.0)]
public class BatchController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<PaginatedList<BatchDto>>> GetBatches([FromQuery] BatchFilter filter)
    {
        var query = new GetBatchsQuery(filter);
        var result = await Mediator.Send(query);
        return result;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BatchDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBatchDetailsQuery() {
            Id = id
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<BatchDto>> CreateBatch([FromBody] CreateBatchCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

}