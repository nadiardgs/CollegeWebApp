using Application.Features.Books.Requests;
using Application.Features.Books.Responses;
using Application.Responses.Books.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateBookResponse>> Create(CreateBookRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    {
        var result = await mediator.Send(new GetAllBooksRequest());
        return Ok(result);
    }
}