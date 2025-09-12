using Application.Batches;
using Application.Batches.Commands.CreateBatch;
using Application.Batches.Queries;
using Application.Batches.Queries.GetBatches;
using Application.Boosters.Queries.GetBatchDetails;
using Application.Common.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Batches;

[ApiVersion(1.0)]
public class BatchController : BaseController
{
    [HttpGet]
    public async Task<Ok<PaginatedList<BatchDto>>> GetBatches([FromQuery] BatchFilter filter)
    {
        var query = new GetBatchsQuery(filter);
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<Ok<BatchDto>> GetBoostOrderDetails(Guid id)
    {
        var query = new GetBatchDetailsQuery
        {
            Id = id
        };
        var result = await Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    [HttpPost]
    public async Task<Ok<BatchDto>> CreateBatch([FromBody] CreateBatchCommand command)
    {
        var result = await Mediator.Send(command);
        return TypedResults.Ok(result);
    }
}