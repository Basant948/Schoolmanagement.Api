using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchoolManagement.Application.Common.Exceptions;

namespace SchoolManagement.Api.ExceptionHandling;

/// <summary>
/// Single place that converts every unhandled exception into an RFC 9457 ProblemDetails response.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Client disconnected - nothing to write, nothing to alarm anyone about.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = 499;
            return true;
        }

        var (status, title, detail) = Map(exception);

        if (status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        else
            logger.LogWarning("Handled {ExceptionType} on {Method} {Path}: {Message}",
                exception.GetType().Name, httpContext.Request.Method, httpContext.Request.Path, exception.Message);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validation)
            problem.Extensions["errors"] = validation.Errors;

        // Only leak internals in Development.
        if (status >= StatusCodes.Status500InternalServerError && environment.IsDevelopment())
            problem.Detail = exception.ToString();

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static (int Status, string Title, string Detail) Map(Exception exception) => exception switch
    {
        ValidationException e => (StatusCodes.Status400BadRequest, "Validation failed", e.Message),
        NotFoundException e => (StatusCodes.Status404NotFound, "Resource not found", e.Message),
        ConflictException e => (StatusCodes.Status409Conflict, "Conflict", e.Message),
        ForbiddenException e => (StatusCodes.Status403Forbidden, "Forbidden", e.Message),
        UnauthorizedException e => (StatusCodes.Status401Unauthorized, "Unauthorized", e.Message),
        EmailDeliveryException e => (StatusCodes.Status502BadGateway, "Email delivery failed", e.Message),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized",
            "Authentication is required to access this resource."),

        BadHttpRequestException e => (e.StatusCode, "Bad request", e.Message),

        DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency conflict",
            "The record was modified by another user. Reload and try again."),
        DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } =>
            (StatusCodes.Status409Conflict, "Duplicate value", "A record with the same unique value already exists."),
        DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation } } =>
            (StatusCodes.Status409Conflict, "Related data conflict",
                "The operation conflicts with related data."),

        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred",
            "Something went wrong. Please try again later.")
    };
}
