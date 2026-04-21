using Application.Exceptions;
using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record EnrollStudentInCourseRequest (int CourseId, int StudentId) : IRequest<StudentEnrollmentDto>;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, StudentEnrollmentDto>
{
    public async Task<StudentEnrollmentDto> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .FirstOrDefaultAsync(s => s.Id == request.CourseId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Course), request.CourseId);
    
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Student), request.StudentId);

        if (course.TeacherId is null or 0)
            throw new NoTeacherAssignedException(request.CourseId);

        var alreadyEnrolled = await context.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId, cancellationToken);

        if (alreadyEnrolled)
            throw new StudentAlreadyEnrolledException(request.StudentId, request.CourseId);

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(cancellationToken);

        return new StudentEnrollmentDto(
            enrollment.Id,
            student.Id,
            student.Name,
            course.Id,
            course.Title);
    }
}