using Application.Features.Courses.Responses;
using Application.Exceptions;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record GetAllCoursesRequest : IRequest<IEnumerable<CourseDto>>;

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
        
        return result ?? throw new CollectionNotFoundException(nameof(Course));
    }
}