using Application.Constants;
using Application.Features.Courses.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Courses;
using Infrastructure.Extensions.Teachers;

namespace Application.Features.Courses.Validators;

public class EnrollTeacherInCourseRequestValidator : AbstractValidator<EnrollTeacherInCourseRequest>
{
    public EnrollTeacherInCourseRequestValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
            .MustAsync((courseId, ct) =>  context.Courses.IdExistsAsync(courseId, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Course), request.CourseId));
                
        RuleFor(request => request.TeacherId)
            .MustAsync((teacherId, ct) => context.Teachers.IdExistsAsync(teacherId, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Teacher), request.TeacherId));
        
        RuleFor(request => request.CourseId)
            .MustAsync(async (courseId, ct) => !await context.Courses.HasTeacherAssignedAsync(courseId, ct))
            .WithMessage(request => ReturnMessages.TeacherAlreadyAssigned(request.CourseId));
    }
}