using Application.Responses.Loans;
using Application.Responses.Loans.DTOs;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Loans;

public class CreateLoanRequestHandler(CollegeDbContext context) : IRequestHandler<CreateLoanRequest, CreateLoanResponse>
{
    public async Task<CreateLoanResponse> Handle(CreateLoanRequest request, CancellationToken ct)
    {
        var loan = new Loan
        {
            StudentId = request.StudentId,
            BookId = request.BookId,
            BorrowedAt = DateTime.UtcNow
        };

        context.Loans.Add(loan);
        await context.SaveChangesAsync(ct);

        return new CreateLoanResponse(new LoanDto(loan.Id, loan.StudentId, loan.BookId));
    }
}