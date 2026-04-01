using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Grades.Responses;
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
    
    [HttpGet("{id:int}/students")]
    public async Task<ActionResult<GetStudentsByCourseIdResponse>> GetStudents(int id)
    {
        var query = new GetStudentsByCourseIdRequest(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost("{courseId:int}/students/enroll")]
    public async Task<ActionResult<EnrollStudentInCourseResponse>> EnrollStudent(int courseId, [FromBody] int studentId)
    {
        await mediator.Send(new EnrollStudentInCourseRequest(courseId, studentId));
        return NoContent();
    }
    
    [HttpPost("{courseId:int}/teachers/enroll")]
    public async Task<ActionResult<EnrollTeacherInCourseResponse>> EnrollTeacher(int courseId, [FromBody] int studentId)
    {
        await mediator.Send(new EnrollTeacherInCourseRequest(courseId, studentId));
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        var result = await mediator.Send(new GetAllCoursesRequest());
        return Ok(result);
    }
    
    [HttpPost("{id:int}/grades/bulk")]
    public async Task<ActionResult> BulkGrade(int id, [FromBody] IReadOnlyCollection<GradeDto> studentGrades)
    {
        await mediator.Send(new BulkGradeStudentsRequest(id, studentGrades));
        return NoContent();
    }
}