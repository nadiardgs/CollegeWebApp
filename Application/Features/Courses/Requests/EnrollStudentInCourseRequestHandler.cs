using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, EnrollStudentInCourseResponse>
{
    public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        var alreadyEnrolled = await context.Courses
            .AnyAsync(c => c.Id == request.CourseId && 
                           c.Students.Any(s => s.Id == request.StudentId), cancellationToken);

        if (alreadyEnrolled) return new EnrollStudentInCourseResponse(true);
        
        var student = new Student { Id = request.StudentId };
        context.Students.Attach(student); 

        var course = await context.Courses.Include(c => c.Students).FirstAsync(c => c.Id == request.CourseId, cancellationToken);
        course.Students.Add(student);

        await context.SaveChangesAsync(cancellationToken);

        return new EnrollStudentInCourseResponse(true);
    }
}