using Application.Requests.Students;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(CollegeDbContext context, IValidator<Student> validator, IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateStudentRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}