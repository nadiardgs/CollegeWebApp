using Application.Constants;
using Application.Features.Students.Responses;
using Application.Models;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public class GetAllStudentsRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllStudentsRequest, ApiResult<IEnumerable<StudentDto>>>
{
    public async Task<ApiResult<IEnumerable<StudentDto>>> Handle(GetAllStudentsRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Students
            .AsNoTracking()
            .Select(s => new StudentDto(s.Id, s.Name))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result.Count == 0 
            ? ApiResult<IEnumerable<StudentDto>>.Empty(ReturnMessages.CollectionNotFound(nameof(Student))) 
            : ApiResult<IEnumerable<StudentDto>>.SuccessResult(result, ReturnMessages.Success(result.Count, nameof(Student)));
    }
}