using Application.Constants;
using Application.Features.Students.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;

namespace Application.Features.Students.Validators;

public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
{
    public UpdateStudentRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage(ReturnMessages.MinLength(nameof(Student)));
    }
}