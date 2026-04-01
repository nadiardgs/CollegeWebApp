using Application.Features.Courses.Responses;
using Application.Exceptions;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public class GetAllCoursesRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllCoursesRequest, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetAllCoursesRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Courses
            .AsNoTracking()
            .Select(c => new CourseDto(
                c.Id, 
                c.Title))
            .ToListAsync(cancellationToken);
        
        return result ?? throw new NotFoundException($"No course was found.");
    }
}