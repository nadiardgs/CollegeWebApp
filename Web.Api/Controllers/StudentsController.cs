using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly CollegeDbContext _context;
    private readonly IValidator<Student> _validator;

    public StudentsController(CollegeDbContext context, IValidator<Student> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult> Create(Student student)
    {
        // Explicit async validation
        var validationResult = await _validator.ValidateAsync(student);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return Ok(student);
    }
}