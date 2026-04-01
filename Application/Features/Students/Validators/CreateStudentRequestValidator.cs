using Application.Constants;
using Application.Entities.Students.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    private readonly CollegeDbContext _context;

    public CreateStudentRequestValidator(CollegeDbContext context)
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