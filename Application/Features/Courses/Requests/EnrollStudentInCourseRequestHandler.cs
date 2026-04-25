using Application.Exceptions;
using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Extensions.Courses;
using Infrastructure.Extensions.Enrollments;
using Infrastructure.Extensions.Students;
using MediatR;

namespace Application.Features.Courses.Requests;

public record EnrollStudentInCourseRequest (int CourseId, int StudentId) : IRequest<StudentEnrollmentDto>;

public class EnrollStudentInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollStudentInCourseRequest, StudentEnrollmentDto>
{
    public async Task<StudentEnrollmentDto> Handle(EnrollStudentInCourseRequest request, CancellationToken cancellationToken)
    {
        if (!await context.Students.IdExistsAsync(request.StudentId, cancellationToken))
            throw new EntityNotFoundException(nameof(Student), request.StudentId);

        if (!await context.Courses.IdExistsAsync(request.CourseId, cancellationToken))
            throw new EntityNotFoundException(nameof(Course), request.CourseId);

        if (!await context.Courses.HasTeacherAssignedAsync(request.CourseId, cancellationToken))
            throw new NoTeacherAssignedException(request.CourseId);

        if (await context.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, cancellationToken))
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
            enrollment.Student.Id,
            enrollment.Student.Name,
            enrollment.Course.Id,
            enrollment.Course.Title);
    }
}