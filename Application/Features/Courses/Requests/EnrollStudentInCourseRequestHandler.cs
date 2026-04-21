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
        var info = await context.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => new
            {
                CourseTitle = c.Title,
                HasTeacher = c.TeacherId != null && c.TeacherId != 0,
                Student = context.Students
                    .Where(s => s.Id == request.StudentId)
                    .Select(s => new { s.Name })
                    .FirstOrDefault(),
                IsAlreadyEnrolled = context.Enrollments
                    .Any(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (info == null) 
            throw new EntityNotFoundException(nameof(Course), request.CourseId);
    
        if (info.Student == null) 
            throw new EntityNotFoundException(nameof(Student), request.StudentId);
    
        if (info.IsAlreadyEnrolled) 
            throw new StudentAlreadyEnrolledException(request.StudentId, request.CourseId);
    
        if (!info.HasTeacher) 
            throw new NoTeacherAssignedException(request.CourseId);
        
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
            request.StudentId,
            info.Student.Name,
            request.CourseId,
            info.CourseTitle);
    }
}