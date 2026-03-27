using Application.Exceptions;
using Application.Responses.Courses;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Courses;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, EnrollStudentInCourseResponse>
{
    public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses
                         .Include(c => c.Students)
                         .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken)
                     ?? throw new NotFoundException($"Course {request.CourseId} not found.");

        var student = await context.Students
                          .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken)
                      ?? throw new NotFoundException($"Student {request.StudentId} not found.");


        var result = new EnrollStudentInCourseResponse(true);
        if (course.Students.Any(s => s.Id == request.StudentId))
            return result;

        course.Students.Add(student);
        
        await context.SaveChangesAsync(cancellationToken);
        return result;
    }
}