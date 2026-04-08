using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record CreateCourseRequest(string Title) : IRequest<CreateCourseResponse>;

public class CreateCourseRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateCourseRequest, CreateCourseResponse>
{
    public async Task<CreateCourseResponse> Handle(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Title = request.Title
        };
        
        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);
        
        var result = await context.Courses
            .Include(g => g.Teacher)
            .FirstAsync(g => g.Id == course.Id, ct);

        return new CreateCourseResponse(result.Id, result.Title);
    }
}