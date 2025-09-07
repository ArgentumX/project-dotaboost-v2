using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => 
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    
    internal int UserId => !User.Identity.IsAuthenticated 
        ? 0 
        : int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
}