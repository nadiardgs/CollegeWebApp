using Application.Requests.Courses;
using Application.Responses.Courses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Courses;

public class CreateCourseRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateCourseRequest, CreateCourseResponse>
{
    public async Task<CreateCourseResponse> Handle(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Title = request.Title,
            TeacherId = request.TeacherId
        };
        
        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);
        
        var result = await context.Courses
            .Include(g => g.Teacher)
            .FirstAsync(g => g.Id == course.Id, ct);

        return new CreateCourseResponse(result.Id, result.Title, result.Teacher.Name);
    }
}