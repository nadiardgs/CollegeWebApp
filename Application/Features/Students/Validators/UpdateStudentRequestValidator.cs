using Application.Constants;
using Application.Features.Students.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Students;

namespace Application.Features.Students.Validators;

public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
{
    public UpdateStudentRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync((id, ct) => context.Students.IdExistsAsync(id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Student), request.Id));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage(ReturnMessages.MinLength(nameof(Student)))
            .MustAsync((request, id, ct) => context.Students.IsNameUniqueAsync(request.Name, request.Id, ct))
            .WithMessage(request => ReturnMessages.UniqueName(nameof(Student), request.Name));
    }
}