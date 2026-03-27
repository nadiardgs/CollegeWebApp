using Application.Requests.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateCourseRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}