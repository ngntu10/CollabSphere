using CollabSphere.Common;
using CollabSphere.Entities.Exceptions;

using Microsoft.AspNetCore.Diagnostics;

using Newtonsoft.Json;

namespace CollabSphere.Exceptions;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await HandleException(httpContext, exception, cancellationToken);

        return true;
    }

    private Task HandleException(HttpContext context, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex.Message);

        var code = StatusCodes.Status500InternalServerError;
        var errors = new List<string> { ex.Message };

        code = ex switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            UnprocessableRequestException => StatusCodes.Status422UnprocessableEntity,
            _ => code
        };

        if (code == StatusCodes.Status500InternalServerError)
        {
            errors = new List<string> { "An error occurred while processing your request" };
        }

        var result = JsonConvert.SerializeObject(ApiResponse<string>.Failure(code, errors));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 200;

        return context.Response.WriteAsync(result, cancellationToken);
    }
}
