using Application.Constants;
using Application.Features.Courses.Responses;
using Application.Models;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record GetAllCoursesRequest : IRequest<ApiResult<IEnumerable<CourseDto>>>;

public class GetAllCoursesRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllCoursesRequest, ApiResult<IEnumerable<CourseDto>>>
{
    public async Task<ApiResult<IEnumerable<CourseDto>>> Handle(GetAllCoursesRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Courses
            .AsNoTracking()
            .Select(c => new CourseDto(
                c.Id, 
                c.Title))
            .ToListAsync(cancellationToken);
        
        var message = result.Count != 0
            ? ReturnMessages.Success(result.Count, nameof(Course)) 
            : ReturnMessages.CollectionNotFound(nameof(CourseDto));

        return new ApiResult<IEnumerable<CourseDto>>(result, message);
    }
}