using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ValidationException = FluentValidation.ValidationException;

namespace WebApplication3;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Failed";
                problemDetails.Detail = "One or more validation errors occurred.";
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());
                break;
            case DbUpdateException { InnerException: PostgresException { SqlState: "23505" } pgEx }:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Duplicate Record Found";
                problemDetails.Detail = GetConstraintMessage(pgEx.ConstraintName);
                break;
            case EntityNotFoundException notFoundException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = notFoundException.Message;
                break;
            default:
                return false;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }

    private static string GetConstraintMessage(string? constraintName)
    {
        return constraintName switch
        {
            "IX_Grades_StudentId_CourseId" => "This student already has a grade for this course.",
            "IX_Courses_Title_TeacherId" => "This teacher is already assigned to a course with this title.",
            _ => "A record with these details already exists in the system."
        };
    }
}