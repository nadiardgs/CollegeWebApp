using Application.Responses.Grades;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Grades;

public class CreateGradeRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateGradeRequest, CreateGradeResponse>
{
    public async Task<CreateGradeResponse> Handle(CreateGradeRequest request, CancellationToken ct)
    {
        var grade = new Grade
        {
            Value = request.Value,
            StudentId = request.StudentId,
            CourseId = request.CourseId
        };
        
        context.Grades.Add(grade);
        await context.SaveChangesAsync(ct);
        
        var result = await context.Grades
            .Include(g => g.Student)
            .Include(g => g.Course)
            .FirstAsync(g => g.Id == grade.Id, ct);

        return new CreateGradeResponse(result.Id, result.Value, result.Student.Name, result.Course.Title);
    }


}