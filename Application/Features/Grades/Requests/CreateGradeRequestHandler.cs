using Application.Features.Grades.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Grades.Requests;

public record CreateGradeRequest(decimal Value, int CourseId, int StudentId) : IRequest<CreateGradeResponse>;

public class CreateGradeRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateGradeRequest, CreateGradeResponse>
{
    public async Task<CreateGradeResponse> Handle(CreateGradeRequest request, CancellationToken ct)
    {
        var grade = new Grade
        {
            Value = request.Value,
            Enrollment = new Enrollment
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            }
        };
        
        context.Grades.Add(grade);
        await context.SaveChangesAsync(ct);
        
        var result = await context.Grades.Include(g => g.Enrollment)
            .ThenInclude(enrollment => enrollment.Student).Include(g => g.Enrollment)
            .ThenInclude(enrollment => enrollment.Course)
            .FirstAsync(g => g.Id == grade.Id, ct);

        return new CreateGradeResponse(result.Id, result.Value, result.Enrollment.Student.Name, result.Enrollment.Course.Title);
    }
}