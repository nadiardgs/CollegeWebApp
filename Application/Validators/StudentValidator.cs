using Application.Constants;
using Application.Requests.Students;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

namespace Application.Validators;

public class StudentValidator : AbstractValidator<CreateStudentRequest>
{
    private readonly CollegeDbContext _context;

    public StudentValidator(CollegeDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3).WithMessage(ValidationMessages.StudentNameMinLength)
            .MustAsync(BeUniqueName).WithMessage(ValidationMessages.StudentAlreadyExists);
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Students.AnyAsync(c => c.Name == name, cancellationToken);
    }
}