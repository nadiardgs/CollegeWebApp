using Application.Features.Loans.Responses;
using MediatR;

namespace Application.Features.Loans.Requests;

public record GetLoanByIdRequest(int Id) : IRequest<LoanDto>;