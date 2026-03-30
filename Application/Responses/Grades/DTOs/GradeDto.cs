namespace Application.Responses.Grades.DTOs;

public record GradeDto(
    string CourseTitle,
    string TeacherName,
    int StudentId,
    decimal? Value
);