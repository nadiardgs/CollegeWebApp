using Application.Exceptions;
using Application.Requests.Books;
using Application.Responses.Books.DTOs;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Requests;

public class GetAllBooksRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllBooksRequest, IEnumerable<BookDto>>
{
    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Books.
            AsNoTracking()
            .Select(b => new BookDto(b.Id, b.Title))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result ?? throw new CollectionNotFoundException(nameof(Book));
    }
}