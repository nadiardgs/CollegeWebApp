using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class GradeValidator : AbstractValidator<Grade>
{
    private readonly CollegeDbContext _context;

    public GradeValidator(CollegeDbContext context)
    {
        _context = context;

        RuleFor(x => x.Value)
            .InclusiveBetween(0, 10).WithMessage("Grades must be between 0 and 10.");

        RuleFor(x => x)
            .MustAsync(BeEnrolledInCourse)
            .WithMessage("Cannot assign a grade to a student who is not enrolled in this course.");
    }

    private async Task<bool> BeEnrolledInCourse(Grade grade, CancellationToken ct)
    {
        return await _context.Courses
            .AnyAsync(c => c.Id == grade.CourseId && 
                           c.Students.Any(s => s.Id == grade.StudentId), ct);
    }
}