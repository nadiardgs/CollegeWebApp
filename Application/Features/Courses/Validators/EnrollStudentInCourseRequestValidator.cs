using Application.Constants;
using Application.Features.Courses.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Courses;
using Infrastructure.Extensions.Enrollments;
using Infrastructure.Extensions.Students;

namespace Application.Features.Courses.Validators;

public class EnrollStudentInCourseRequestValidator : AbstractValidator<EnrollStudentInCourseRequest>
{
    public EnrollStudentInCourseRequestValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
            .MustAsync((id, ct) => context.Courses.IdExistsAsync(id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Course), request.CourseId));
        
        RuleFor(request => request.StudentId)
            .MustAsync((id, ct) => context.Students.IdExistsAsync(id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Student), request.StudentId));
        
        RuleFor(request => request)
            .MustAsync(async (request, ct) => 
                !await context.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, ct)) 
            .WithMessage(request => ReturnMessages.AlreadyEnrolled(request.StudentId, request.CourseId));
        
        RuleFor(request => request.CourseId)
            .MustAsync((courseId, ct) => context.Courses.HasTeacherAssignedAsync(courseId, ct))
            .WithMessage(request => ReturnMessages.NoTeacherAssigned(request.CourseId));
    }
}