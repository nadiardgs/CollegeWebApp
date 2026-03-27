using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController(CollegeDbContext context, IValidator<Teacher> validator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(Teacher teacher)
    {
        var validationResult = await validator.ValidateAsync(teacher);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
        return Ok(teacher);
    }
}