using Application.Constants;
using Application.Features.Grades.Requests;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Enrollments;

namespace Application.Features.Grades.Validators;

public class CreateGradeRequestValidator : AbstractValidator<CreateGradeRequest>
{
    public CreateGradeRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.Value)
            .InclusiveBetween(0, 10)
            .WithMessage(ReturnMessages.GradeOutOfRange());

        RuleFor(x => x)
            .MustAsync((grade, ct) => context.Enrollments.IsEnrolledAsync(grade.StudentId, grade.CourseId, ct))
            .WithMessage(request => ReturnMessages.AlreadyEnrolled(request.StudentId, request.CourseId));
    }
}