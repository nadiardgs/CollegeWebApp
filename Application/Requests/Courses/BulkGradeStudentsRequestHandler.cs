using Application.Exceptions;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Courses;

public class BulkGradeStudentsRequestHandler(CollegeDbContext context) 
    : IRequestHandler<BulkGradeStudentsRequest, bool>
{
    public async Task<bool> Handle(BulkGradeStudentsRequest request, CancellationToken ct)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, ct);
        if (!courseExists) throw new NotFoundException($"Course {request.CourseId} not found.");

        var studentIds = request.StudentGrades.Keys.ToList();
        var existingGrades = await context.Grades
            .Where(g => g.CourseId == request.CourseId && studentIds.Contains(g.StudentId))
            .ToListAsync(ct);

        foreach (var (studentId, gradeValue) in request.StudentGrades)
        {
            var existing = existingGrades.FirstOrDefault(g => g.StudentId == studentId);

            if (existing != null)
            {
                existing.Value = gradeValue;
            }
            else
            {
                context.Grades.Add(new Grade 
                { 
                    CourseId = request.CourseId, 
                    StudentId = studentId, 
                    Value = gradeValue 
                });
            }
        }

        await context.SaveChangesAsync(ct);
        return true;
    }
}