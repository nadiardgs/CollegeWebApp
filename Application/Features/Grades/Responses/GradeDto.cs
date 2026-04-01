namespace Application.Features.Grades.Responses;

public record GradeDto(
    string CourseTitle,
    string TeacherName,
    int StudentId,
    decimal? Value
);