using Application.Exceptions;
using Application.Responses.Courses.DTOs;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Courses;

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