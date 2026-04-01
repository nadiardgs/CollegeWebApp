using Application.Entities.Students.Requests;
using Application.Exceptions;
using Application.Features.Students.Responses;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public class GetAllStudentsRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllStudentsRequest, IEnumerable<StudentDto>>
{
    public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Students
            .AsNoTracking()
            .Select(s => new StudentDto(s.Id, s.Name))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result ?? throw new NotFoundException($"No student was found.");
    }
}