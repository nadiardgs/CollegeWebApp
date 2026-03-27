using Application.Requests.Teachers;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class TeacherValidator : AbstractValidator<CreateTeacherRequest>
{
    private readonly CollegeDbContext _context;

    public TeacherValidator(CollegeDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(BeUniqueName).WithMessage("A teacher with this name already exists.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Teachers.AnyAsync(s => s.Name == name, cancellationToken);
    }
}