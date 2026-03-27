namespace Application.Responses.Courses;

public record GetStudentsByCourseIdResponse(string CourseName, string TeacherName, IReadOnlyCollection<string> Students);