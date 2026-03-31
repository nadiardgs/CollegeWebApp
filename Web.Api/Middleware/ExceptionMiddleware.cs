using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ValidationException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        
        Console.WriteLine($"--- ERROR JSON: {exception.Message} ---");

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = exception.Message,
            Detail = exception.StackTrace, //note:  ok for development. never send stack trace in production
            Instance = context.Request.Path
        };

        if (exception is ValidationException valEx)
        {
            problemDetails.Extensions.Add("errors", valEx.Errors);
            Console.WriteLine($"--- ERROR VALIDATION: {exception.Message} ---");

        }

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}