namespace Application.Responses.Grades;

public record GradeDto(
    string CourseTitle,
    string TeacherName,
    decimal? Value
);