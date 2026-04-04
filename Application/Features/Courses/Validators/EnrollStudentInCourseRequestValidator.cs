using Application.Constants;
using Application.Features.Courses.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Validators;

public class CourseRequestValidator : AbstractValidator<EnrollStudentInCourseRequest>
{
    public CourseRequestValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
            .MustAsync(async (id, ct) => await context.Courses.AnyAsync(c => c.Id == id, ct))
            .WithMessage(request => ErrorMessages.EntityNotFound(nameof(Course), request.CourseId));
        
        RuleFor(request => request.StudentId)
            .MustAsync(async (id, ct) => await context.Students.AnyAsync(s => s.Id == id, ct))
            .WithMessage(request => ErrorMessages.EntityNotFound(nameof(Student), request.StudentId));
    }
}