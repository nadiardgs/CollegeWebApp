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
            .MustAsync((request, ct) => 
                context.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, ct))
            .WithMessage(req => ReturnMessages.NotEnrolled(req.StudentId, req.CourseId));
    }
}