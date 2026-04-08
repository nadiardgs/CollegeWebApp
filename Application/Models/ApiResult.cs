namespace Application.Models;

public record ApiResult<T>(T Data, string Message = "");