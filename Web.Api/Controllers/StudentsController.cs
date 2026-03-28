using Application.Requests.Students;
using Application.Responses.Students;
using Application.Responses.Students.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateStudentRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetStudentByIdResponse>> GetById(int id)
    {
        var query = new GetStudentByIdRequest(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        var query = new GetAllStudentsRequest();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}