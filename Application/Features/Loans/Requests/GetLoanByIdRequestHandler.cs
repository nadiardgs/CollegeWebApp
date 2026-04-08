using Application.Exceptions;
using Application.Features.Loans.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Loans.Requests;

public record GetLoanByIdRequest(int Id) : IRequest<LoanDto>;

public class GetLoanByIdRequestHandler(CollegeDbContext context) : IRequestHandler<GetLoanByIdRequest, LoanDto>
{
    public async Task<LoanDto> Handle(GetLoanByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Loans
            .AsNoTracking()
            .Where(l => l.Id == request.Id)
            .Select (l => new LoanDto(
                l.Id,
                l.StudentId,
                l.BookId))
            .FirstOrDefaultAsync(cancellationToken);
        
        return result ?? throw new EntityNotFoundException(nameof(Loan), request.Id);
    }
}