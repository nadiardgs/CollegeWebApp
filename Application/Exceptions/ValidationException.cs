namespace Application.Exceptions;

public abstract class ValidationException(IDictionary<string, string[]> errors) 
    : Exception("One or more validation failures have occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}