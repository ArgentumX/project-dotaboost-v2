using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    internal Guid UserId => !User.Identity.IsAuthenticated
        ? Guid.Empty
        : Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
}