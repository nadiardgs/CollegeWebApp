using Application.Features.Grades.Requests;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateGradeRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}