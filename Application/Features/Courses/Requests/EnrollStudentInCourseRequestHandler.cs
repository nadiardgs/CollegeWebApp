using Application.Constants;
using Application.Exceptions;
using Application.Features.Courses.Responses;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, EnrollStudentInCourseResponse>
{
    public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses
                         .Include(c => c.Students) 
                         .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken)
                     ?? throw new NotFoundException(ValidationMessages.CourseNotFound);

        var student = await context.Students.FindAsync([request.StudentId], cancellationToken)
                      ?? throw new NotFoundException(ValidationMessages.StudentNotFound);

        if (course.Students.Any(s => s.Id == request.StudentId)) return new EnrollStudentInCourseResponse(true);
        
        course.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        return new EnrollStudentInCourseResponse(true);
    }
}