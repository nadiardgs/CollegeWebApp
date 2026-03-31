using Application.Responses.Loans;
using MediatR;

namespace Application.Requests.Loans;

public record CreateLoanRequest(int StudentId, int BookId) : IRequest<CreateLoanResponse>;