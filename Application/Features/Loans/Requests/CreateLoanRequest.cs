using Application.Features.Loans.Responses;
using MediatR;

namespace Application.Features.Loans.Requests;

public record CreateLoanRequest(int StudentId, int BookId) : IRequest<CreateLoanResponse>;