using Application.Requests.Teachers;
using Application.Responses.Teachers.DTOs;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateTeacherRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll()
    {
        var result = await mediator.Send(new GetAllTeachersRequest());
        return Ok(result);
    }
}