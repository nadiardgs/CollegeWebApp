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
        
        var enrollments = await context.Enrollments
            .Include(e => e.Grades)
            .Where(e => e.CourseId == request.CourseId && studentIds.Contains(e.StudentId))
            .ToListAsync(ct);

        var validGradeDtos = request.Grades
            .Where(g => g.Value.HasValue)
            .ToList();

        foreach (var gradeDto in validGradeDtos)
        {
            var incomingValue = gradeDto.Value!.Value;
            
            var enrollment = enrollments.FirstOrDefault(e => e.StudentId == gradeDto.StudentId);

            if (enrollment == null)
            {
                continue; 
            }

            var existingGrade = enrollment.Grades.FirstOrDefault();

            if (existingGrade != null)
            {
                existingGrade.Value = incomingValue;
            }
            else
            {
                context.Grades.Add(new Grade
                {
                    EnrollmentId = enrollment.Id,
                    Value = incomingValue
                });
            }
        }

        await context.SaveChangesAsync(ct);
        return true;
    }
}