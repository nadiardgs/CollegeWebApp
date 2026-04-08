using Application.Constants;
using Application.Features.Teachers.Responses;
using Application.Models;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Teachers.Requests;

public class GetAllTeachersRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllTeachersRequest, ApiResult<IEnumerable<TeacherDto>>>
{
    public async Task<ApiResult<IEnumerable<TeacherDto>>> Handle(GetAllTeachersRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Teachers
            .AsNoTracking()
            .Select(s => new TeacherDto(s.Id, s.Name))
            .ToListAsync(cancellationToken: cancellationToken);
        
        var message = result.Count != 0
            ? ReturnMessages.Success(result.Count, nameof(Teacher)) 
            : ReturnMessages.CollectionNotFound(nameof(Teacher));

        return new ApiResult<IEnumerable<TeacherDto>>(result, message);
    }
}