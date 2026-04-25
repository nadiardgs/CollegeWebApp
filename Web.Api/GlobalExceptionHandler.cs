using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Exceptions;

namespace WebApplication3;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            UniqueNameException => HttpStatusCode.Conflict,
            EntityNotFoundException => HttpStatusCode.NotFound,
            CollectionNotFoundException => HttpStatusCode.NotFound,
            NoTeacherAssignedException => HttpStatusCode.Conflict,
            TeacherAlreadyAssignedException => HttpStatusCode.Conflict,
            StudentAlreadyEnrolledException => HttpStatusCode.Conflict,
            FluentValidation.ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        
        var details = exception.Message;

        if (exception is FluentValidation.ValidationException valEx)
        {
            details = valEx.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed";
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = "An error occurred",
            Detail = details,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails, ct);

        return true; 
    }
}