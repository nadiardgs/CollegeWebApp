using Application.Features.Books.Responses;
using Application.Responses.Books;
using MediatR;

namespace Application.Features.Books.Requests;

public record CreateBookRequest(string Title) : IRequest<CreateBookResponse>;