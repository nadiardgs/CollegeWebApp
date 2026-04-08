using Application.Features.Grades.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Grades.Validators;

public class CreateGradeRequestValidator : AbstractValidator<CreateGradeRequest>
{
    private readonly CollegeDbContext _context;

    public CreateGradeRequestValidator(CollegeDbContext context)
    {
        _context = context;

        RuleFor(x => x.Value)
            .InclusiveBetween(0, 10).WithMessage("Grades must be between 0 and 10.");

        RuleFor(x => x)
            .MustAsync(BeEnrolledAsync)
            .WithMessage("Cannot assign a grade to a student who is not enrolled in this course.");
    }

    private async Task<bool> BeEnrolledAsync(CreateGradeRequest request, CancellationToken ct)
    {
        return await _context.Enrollments.AnyAsync(e => 
                e.StudentId == request.StudentId && 
                e.CourseId == request.CourseId, 
            ct);
    }
}