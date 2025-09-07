using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using FluentValidation;

namespace WebApi.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
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
        }
        
        if (code == HttpStatusCode.InternalServerError)
        {
            Console.WriteLine(exception.Message);
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}