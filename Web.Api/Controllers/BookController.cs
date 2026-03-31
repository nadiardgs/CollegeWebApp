using Application.Requests.Books;
using Application.Responses.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateBookResponse>> Create(CreateBookRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}