using Application.Features.Courses.Responses;
using Application.Exceptions;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record GetStudentsByCourseIdRequest(int CourseId): IRequest<GetStudentsByCourseIdResponse>;

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
                c.Enrollments.Select(s => s.Student.Name).ToList()
            )).AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? throw new EntityNotFoundException(nameof(Course), request.CourseId);
    }
}