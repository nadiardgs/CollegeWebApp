using Application.Constants;
using Application.Features.Loans.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.StudentId)
            .MustAsync(async (id, ct) => await context.Students.AnyAsync(s => s.Id == id, ct))
            .WithMessage(ValidationMessages.StudentNotFound);

        RuleFor(x => x.BookId)
            .MustAsync(async (id, ct) => await context.Books.AnyAsync(b => b.Id == id, ct))
            .WithMessage(ValidationMessages.BookNotFound);
    }
}