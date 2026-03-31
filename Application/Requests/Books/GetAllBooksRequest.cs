using Application.Responses.Books.DTOs;
using MediatR;

namespace Application.Requests.Books;

public record GetAllBooksRequest() : IRequest<IEnumerable<BookDto>>;