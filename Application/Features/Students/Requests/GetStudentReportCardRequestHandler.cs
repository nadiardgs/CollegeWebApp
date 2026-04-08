using Application.Features.Grades.Responses;
using Application.Exceptions;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Extensions.Students;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public record GetStudentReportCardRequest(int StudentId) : IRequest<GetStudentReportCardResponse>;

public class GetStudentReportCardRequestHandler(CollegeDbContext context) 
    : IRequestHandler<GetStudentReportCardRequest, GetStudentReportCardResponse>
{
    public async Task<GetStudentReportCardResponse> Handle(GetStudentReportCardRequest request, CancellationToken ct)
    {
        var exists = await context.Students.StudentIdExistsAsync(request.StudentId, ct);
        if (!exists) throw new EntityNotFoundException(nameof(Student),  request.StudentId);
        
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

        return reportCard ?? throw new EntityNotFoundException(nameof(Student), request.StudentId);
    }
}