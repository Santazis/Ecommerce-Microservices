using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.ExceptionHandlers
{
    public sealed class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ValidationException validationException)
            {
                return false;
            }
            logger.LogWarning("Handling ValidationException with {ErrorCount} errors", 
                validationException.Errors.Count());
            var errors = validationException.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Select(e=> e.ErrorMessage).ToArray());
            var problemDetails = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions.Add("errors",errors);
            problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problemDetails, options);
            await httpContext.Response.WriteAsync(json,cancellationToken);
            return true;
        }
    }
}