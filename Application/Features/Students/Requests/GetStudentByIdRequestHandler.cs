using Application.Exceptions;
using Application.Features.Courses.Responses;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public record GetStudentByIdRequest(int Id) : IRequest<GetStudentByIdResponse>;

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
        
        return result ?? throw new EntityNotFoundException(nameof(Student), request.Id);
    }
}