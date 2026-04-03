using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
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
        return CreatedAtAction(
            nameof(GetById), 
            new { id = result.TeacherDto.Id }, 
            result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAll()
    {
        var query = new GetAllTeachersRequest();
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetById(int id)
    {
        var query = new GetTeacherByIdRequest(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}