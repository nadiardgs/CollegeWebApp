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
        var courseExists = await context.Courses.IdExistsAsync(request.CourseId, cancellationToken);
        if (!courseExists) 
            throw new EntityNotFoundException(nameof(Course), request.CourseId);

        var studentExists = await context.Students.IdExistsAsync(request.StudentId, cancellationToken);
        if (!studentExists) 
            throw new EntityNotFoundException(nameof(Student), request.StudentId);

        var alreadyEnrolled = await context.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, cancellationToken);
        if (alreadyEnrolled)
            throw new StudentAlreadyEnrolledException(request.StudentId, request.CourseId);
        
        var noTeacherAssigned = await context.Courses.HasTeacherAssignedAsync(request.CourseId, cancellationToken);
        if (!noTeacherAssigned)
            throw new NoTeacherAssignedException(request.CourseId);

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(cancellationToken);

        var result = new StudentEnrollmentDto(
            enrollment.Id,
            enrollment.Student.Id,
            enrollment.Student.Name,
            enrollment.Course.Id,
            enrollment.Course.Title);

        return result;
    }
}