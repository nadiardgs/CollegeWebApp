using Application.Requests.Courses;
using Application.Responses.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateCourseResponse>> Create(CreateCourseRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
    
    [HttpGet("{id:int}/students")]
    public async Task<ActionResult<GetStudentsByCourseIdResponse>> GetStudents(int id)
    {
        var query = new GetStudentsByCourseIdRequest(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}