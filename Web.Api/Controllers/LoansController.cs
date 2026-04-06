using Application.Features.Loans.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create(CreateLoanRequest request)
    {
        var result = await mediator.Send(request);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Loan.Id },
            result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        var request = new  GetLoanByIdRequest(id);
        var result = await mediator.Send(request);
        return Ok(result);
    }
}