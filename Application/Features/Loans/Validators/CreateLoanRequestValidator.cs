using Application.Constants;
using Application.Features.Loans.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Loans.Validators;

public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator(CollegeDbContext context)
    {
        RuleFor(x => x.StudentId)
            .MustAsync(async (id, ct) => await context.Students.AnyAsync(s => s.Id == id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Student), request.StudentId));

        RuleFor(x => x.BookId)
            .MustAsync(async (id, ct) => await context.Books.AnyAsync(b => b.Id == id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Book), request.BookId));
    }
}