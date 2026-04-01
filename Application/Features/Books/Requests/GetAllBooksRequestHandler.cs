using Application.Exceptions;
using Application.Responses.Books.DTOs;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Books;

public class GetAllBooksRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllBooksRequest, IEnumerable<BookDto>>
{
    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Books.
            AsNoTracking()
            .Select(b => new BookDto(b.Id, b.Title))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result ?? throw new NotFoundException($"No book was found.");
    }
}