using Application.Features.Courses.Responses;
using Application.Exceptions;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public class GetStudentsByCourseIdRequestHandler(CollegeDbContext context)
    : IRequestHandler<GetStudentsByCourseIdRequest, GetStudentsByCourseIdResponse>
{
    public async Task<GetStudentsByCourseIdResponse> Handle(GetStudentsByCourseIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => new GetStudentsByCourseIdResponse(
                c.Title,
                c.Teacher.Name,
                c.Students.Select(s => s.Name).ToList()
            )).AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? throw new NotFoundException($"Course with ID {request.CourseId} not found.");
    }
}