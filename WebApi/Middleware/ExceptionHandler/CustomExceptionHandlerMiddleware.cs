using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using FluentValidation;

namespace WebApi.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }  
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        object result = new { message = "An unexpected error occurred." };

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = new { errors = validationException.Errors };
                break;

            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = new { message = notFoundException.Message };
                break;
            case BadRequestException badRequestException:
                code = HttpStatusCode.BadRequest;
                result = new { message = badRequestException.Message };
                break;
            default:
                _logger.LogError($"Unhandled exception: {exception.Message}");
                break;
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}