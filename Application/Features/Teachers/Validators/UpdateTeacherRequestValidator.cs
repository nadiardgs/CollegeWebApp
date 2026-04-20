using Application.Constants;
using Application.Features.Teachers.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Teachers;

namespace Application.Features.Teachers.Validators;

public class UpdateTeacherRequestValidator : AbstractValidator<UpdateTeacherRequest>
{
    public UpdateTeacherRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync((id, ct) => context.Teachers.IdExistsAsync(id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Teacher), request.Id));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage(ReturnMessages.MinLength(nameof(Teacher)))
            .MustAsync((request, id, ct) => context.Teachers.IsNameUniqueAsync(request.Name, request.Id, ct))
            .WithMessage(request => ReturnMessages.UniqueName(nameof(Teacher), request.Name));
    }
}