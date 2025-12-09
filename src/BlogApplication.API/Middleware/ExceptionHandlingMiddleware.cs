using System.Net;
using System.Text.Json;
using BlogApplication.Domain.Exceptions;

namespace BlogApplication.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case UnauthorizedException:
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case DomainException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            statusCode = (int)statusCode
        });

        return context.Response.WriteAsync(result);
    }
}
