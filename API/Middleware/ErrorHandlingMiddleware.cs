using System.Net;
using System.Text.Json;

namespace IdeonBack.API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocurrió una excepción no controlada");

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ocurrió un error interno en el servidor";

        switch (exception)
        {
            case ArgumentException _:
            case InvalidOperationException _:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case KeyNotFoundException _:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case UnauthorizedAccessException _:
                statusCode = HttpStatusCode.Unauthorized;
                message = "No autorizado";
                break;
        }

        var response = new
        {
            error = message,
            statusCode = (int)statusCode,
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
