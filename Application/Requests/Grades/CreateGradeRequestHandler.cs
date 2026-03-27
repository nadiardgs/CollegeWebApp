using Application.Responses.Grades;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Grades;

public class CreateGradeRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateGradeRequest, CreateGradeResponse>
{
    public async Task<CreateGradeResponse> Handle(CreateGradeRequest request, CancellationToken ct)
    {
        var grade = new Grade
        {
            Value = request.Value,
            Student = request.Student
        };
        
        context.Grades.Add(grade);
        await context.SaveChangesAsync(ct);

        return new CreateGradeResponse(grade.Value, grade.Student);
    }


}