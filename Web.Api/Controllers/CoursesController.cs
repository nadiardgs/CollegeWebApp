using Application.Requests.Courses;
using Application.Responses.Courses;
using Application.Responses.Courses.DTOs;
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
    
    [HttpPost("{courseId:int}/enroll")]
    public async Task<ActionResult<EnrollStudentInCourseResponse>> Enroll(int courseId, [FromBody] int studentId)
    {
        await mediator.Send(new EnrollStudentInCourseRequest(courseId, studentId));
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        var result = await mediator.Send(new GetAllCoursesRequest());
        return Ok(result);
    }
}