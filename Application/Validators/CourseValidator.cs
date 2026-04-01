using Application.Constants;
using Application.Features.Courses.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class CourseValidator : AbstractValidator<EnrollStudentInCourseRequest>
{
    public CourseValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
            .MustAsync(async (id, ct) => await context.Courses.AnyAsync(c => c.Id == id, ct))
            .WithMessage(ValidationMessages.CourseNotFound);
        
        RuleFor(request => request.StudentId)
            .MustAsync(async (id, ct) => await context.Students.AnyAsync(s => s.Id == id, ct))
            .WithMessage(ValidationMessages.StudentNotFound);
    }
}