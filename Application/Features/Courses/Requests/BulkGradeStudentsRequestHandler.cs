using Application.Exceptions;
using Application.Features.Grades.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Requests;

public record BulkGradeStudentsRequest (
    int CourseId, 
    IReadOnlyCollection<GradeDto> Grades
) : IRequest<bool>;

public class BulkGradeStudentsRequestHandler(CollegeDbContext context) 
    : IRequestHandler<BulkGradeStudentsRequest, bool>
{
    public async Task<bool> Handle(BulkGradeStudentsRequest request, CancellationToken ct)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, ct);
        if (!courseExists) 
            throw new EntityNotFoundException(nameof(Course), request.CourseId);

        var studentIds = request.Grades.Select(g => g.StudentId).ToList();

        var existingGrades = await context.Grades
            .Where(g => g.CourseId == request.CourseId && studentIds.Contains(g.StudentId))
            .ToListAsync(ct);
        
        var validGrades = request.Grades
            .Where(g => g.Value.HasValue)
            .ToList();
        
        foreach (var gradeDto in validGrades)
        {
            var incomingValue = gradeDto.Value!.Value;
            
            var existing = existingGrades.FirstOrDefault(g => g.StudentId == gradeDto.StudentId);

            if (existing != null)
            {
                existing.Value = incomingValue;
            }
            else
            {
                context.Grades.Add(new Grade
                {
                    CourseId = request.CourseId,
                    StudentId = gradeDto.StudentId,
                    Value = incomingValue
                });
            }
        }

        await context.SaveChangesAsync(ct);
        
        return true;
    }
}