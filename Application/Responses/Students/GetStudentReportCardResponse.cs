using Application.Responses.Grades;

namespace Application.Responses.Students;

public record GetStudentReportCardResponse(
    int StudentId,
    string StudentName,
    List<GradeDto> Grades
);