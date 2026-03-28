using Application.Responses.Courses.DTOs;

namespace Application.Responses.Students;

public record GetStudentByIdResponse(int Id, string Name, List<CourseDto> EnrolledCourses);