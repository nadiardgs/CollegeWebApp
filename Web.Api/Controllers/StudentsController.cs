using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController(CollegeDbContext context, IValidator<Student> validator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(Student student)
    {
        var validationResult = await validator.ValidateAsync(student);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        context.Students.Add(student);
        await context.SaveChangesAsync();
        return Ok(student);
    }
}