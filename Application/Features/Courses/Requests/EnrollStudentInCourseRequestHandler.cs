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
        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists) 
            throw new EntityNotFoundException(nameof(Course), request.CourseId);

        var studentExists = await context.Students.AnyAsync(s => s.Id == request.StudentId, cancellationToken);
        if (!studentExists) 
            throw new EntityNotFoundException(nameof(Student), request.StudentId);

        var alreadyEnrolled = await context.Enrollments.AnyAsync(e => 
                e.StudentId == request.StudentId && e.CourseId == request.CourseId, 
            cancellationToken);

        if (alreadyEnrolled) 
        {
            return new EnrollStudentInCourseResponse(true);
        }

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(cancellationToken);

        return new EnrollStudentInCourseResponse(true);
    }
}