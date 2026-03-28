using Application.Exceptions;
using Application.Requests.Courses.DTOs;
using Application.Responses.Students;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Students;

public class GetStudentByIdRequestHandler (CollegeDbContext context)
    : IRequestHandler<GetStudentByIdRequest, GetStudentByIdResponse>
{
    public async Task<GetStudentByIdResponse> Handle(GetStudentByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Students
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s=> new GetStudentByIdResponse(
                s.Id,
                s.Name,
                s.Courses.Select(
                    c => new CourseDto(
                        c.Id, 
                        c.Title))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
        
        return result ?? throw new NotFoundException($"Student with ID {request.Id} not found.");
    }
}