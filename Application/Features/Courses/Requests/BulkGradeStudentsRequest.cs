using Application.Features.Grades.Responses;
using MediatR;

namespace Application.Features.Courses.Requests;

public record BulkGradeStudentsRequest (
    int CourseId, 
    IReadOnlyCollection<GradeDto> Grades
) : IRequest<bool>;