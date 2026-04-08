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
        
        var message = result.Count != 0
            ? ReturnMessages.Success(result.Count, nameof(Student)) 
            : ReturnMessages.CollectionNotFound(nameof(Student));

        return new ApiResult<IEnumerable<StudentDto>>(result, message);
    }
}