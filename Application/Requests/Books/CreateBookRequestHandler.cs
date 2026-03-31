using Application.Responses.Books;
using Application.Responses.Books.DTOs;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Books;

public class CreateBookRequestHandler (CollegeDbContext context)
    : IRequestHandler<CreateBookRequest, CreateBookResponse>
{
    public async Task<CreateBookResponse> Handle(CreateBookRequest request, CancellationToken cancellationToken)
    {
        var book = new Book { Title = request.Title };
        
        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateBookResponse(
            new BookDto(
                book.Id,
                book.Title)
        );
    }
}