using Application.Responses.Grades.DTOs;
using MediatR;

namespace Application.Requests.Courses;

public record BulkGradeStudentsRequest (
    int CourseId, 
    IReadOnlyCollection<GradeDto> Grades
) : IRequest<bool>;