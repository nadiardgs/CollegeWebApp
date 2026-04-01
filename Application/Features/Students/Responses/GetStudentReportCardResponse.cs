using Application.Features.Grades.Responses;

namespace Application.Features.Students.Responses;

public record GetStudentReportCardResponse(
    int StudentId,
    string StudentName,
    List<GradeDto> Grades
);