using Application.Constants;
using Application.Features.Courses.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Validators;

public class EnrollTeacherInCourseRequestValidator : AbstractValidator<EnrollTeacherInCourseRequest>
{
    public EnrollTeacherInCourseRequestValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
                .MustAsync(async (id, ct) => await context.Courses.AnyAsync(c => c.Id == id, ct))
            .WithMessage(ValidationMessages.CourseNotFound);
                
            RuleFor(request => request.TeacherId)
                .MustAsync(async (id, ct) => await context.Teachers.AnyAsync(s => s.Id == id, ct))
            .WithMessage(ValidationMessages.TeacherNotFound);
    }
    
}