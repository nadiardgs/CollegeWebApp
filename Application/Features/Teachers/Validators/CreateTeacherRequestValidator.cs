using Application.Constants;
using Application.Features.Teachers.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Teachers.Validators;

public class CreateTeacherRequestValidator : AbstractValidator<CreateTeacherRequest>
{
    private readonly CollegeDbContext _context;

    public CreateTeacherRequestValidator(CollegeDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(BeUniqueName).WithMessage(request => ErrorMessages.UniqueName(nameof(Teacher), request.Name));
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Teachers.AnyAsync(s => s.Name == name, cancellationToken);
    }
}