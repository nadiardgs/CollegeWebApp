using Application.Exceptions;
using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record EnrollStudentInCourseRequest (int CourseId, int StudentId) : IRequest<EnrollStudentInCourseResponse>;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, EnrollStudentInCourseResponse>
{
    public async Task<EnrollStudentInCourseResponse> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses
                         .Include(c => c.Students) 
                         .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken)
                     ?? throw new EntityNotFoundException(nameof(Course), request.CourseId);

        var student = await context.Students.FindAsync([request.StudentId], cancellationToken)
                      ?? throw new EntityNotFoundException(nameof(Student), request.StudentId);

        if (course.Students.Any(s => s.Id == request.StudentId)) return new EnrollStudentInCourseResponse(true);
        
        course.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        return new EnrollStudentInCourseResponse(true);
    }
}