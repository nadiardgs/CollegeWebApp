using Application.Responses.Grades.DTOs;

namespace Application.Responses.Students;

public record GetStudentReportCardResponse(
    int StudentId,
    string StudentName,
    List<GradeDto> Grades
);