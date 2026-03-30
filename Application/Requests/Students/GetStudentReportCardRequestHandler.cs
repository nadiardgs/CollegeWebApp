using Application.Exceptions;
using Application.Responses.Grades.DTOs;
using Application.Responses.Students;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Students;

public class GetStudentReportCardRequestHandler(CollegeDbContext context) 
    : IRequestHandler<GetStudentReportCardRequest, GetStudentReportCardResponse>
{
    public async Task<GetStudentReportCardResponse> Handle(GetStudentReportCardRequest request, CancellationToken ct)
    {
        var reportCard = await context.Students
            .AsNoTracking()
            .Where(s => s.Id == request.StudentId)
            .Select(s => new GetStudentReportCardResponse(
                s.Id,
                s.Name,
                s.Courses.Select(course => new GradeDto(
                    course.Title,
                    course.Teacher.Name,
                    s.Id,
                    context.Grades
                        .Where(g => g.StudentId == s.Id && g.CourseId == course.Id)
                        .Select(g => (decimal?)g.Value)
                        .FirstOrDefault()
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return reportCard ?? throw new NotFoundException($"Student {request.StudentId} not found.");
    }
}