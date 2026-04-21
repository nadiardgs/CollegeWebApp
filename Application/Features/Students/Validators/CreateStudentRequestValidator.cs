using Application.Constants;
using Application.Features.Students.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Students;

namespace Application.Features.Students.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator(CollegeDbContext dbContext)
    {
        var context = dbContext;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage(ReturnMessages.MinLength(nameof(Student)))
            .MustAsync(async (name, ct) => await context.Students.IsNameUniqueAsync(name, null, ct))
            .WithMessage(request => ReturnMessages.UniqueName(nameof(Student), request.Name));
    }
    
}