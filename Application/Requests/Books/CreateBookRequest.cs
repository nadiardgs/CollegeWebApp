using Application.Responses.Books;
using MediatR;

namespace Application.Requests.Books;

public record CreateBookRequest(string Title) : IRequest<CreateBookResponse>;