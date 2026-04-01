namespace Application.Features.Courses.Responses;

public record GetStudentsByCourseIdResponse(string CourseName, string TeacherName, IReadOnlyCollection<string> Students);