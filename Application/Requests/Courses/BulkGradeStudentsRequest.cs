using MediatR;

namespace Application.Requests.Courses;

public record BulkGradeStudentsRequest (
    int CourseId, 
    Dictionary<int, decimal> StudentGrades
) : IRequest<bool>;