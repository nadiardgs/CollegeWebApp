using Application.Requests.Teachers;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController(CollegeDbContext context, IValidator<Teacher> validator, IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateTeacherRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}