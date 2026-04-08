namespace Application.Models;

public record ApiResult<T>(
    T Data, 
    string? Message = "", 
    bool Success = true)
{
    public static ApiResult<T> Empty(string message) 
        => new(default!, message, true);
    
    public static ApiResult<T> SuccessResult(T data, string message = "") 
        => new(data, message, true);
}