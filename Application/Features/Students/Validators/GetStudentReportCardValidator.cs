using Application.Constants;
using Application.Features.Students.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Students;

namespace Application.Features.Students.Validators;

public class GetStudentReportCardValidator : AbstractValidator<GetStudentReportCardRequest>
{
    public GetStudentReportCardValidator(CollegeDbContext context)
    {
        RuleFor(x => x.StudentId)
            .MustAsync((id, ct) => context.Students.IdExistsAsync(id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Student), request.StudentId));
    }
}