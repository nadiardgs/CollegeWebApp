using Application.Constants;
using Application.Features.Teachers.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Teachers;

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
            .WithMessage(ReturnMessages.MinLength(nameof(Teacher)))
            .MustAsync((name, ct) => context.Teachers.IsNameUniqueAsync(name, ct))
            .WithMessage(request => ReturnMessages.UniqueName(nameof(Teacher), request.Name));
    }
}