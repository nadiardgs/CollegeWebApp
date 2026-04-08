using Application.Constants;
using Application.Features.Courses.Requests;
using Domain.Entities;
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
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Course), request.CourseId));
                
            RuleFor(request => request.TeacherId)
                .MustAsync(async (id, ct) => await context.Teachers.AnyAsync(s => s.Id == id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Teacher), request.TeacherId));
    }
    
}