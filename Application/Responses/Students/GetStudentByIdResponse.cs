using Application.Requests.Courses.DTOs;

namespace Application.Responses.Students;

public record GetStudentByIdResponse(int Id, string Name, List<CourseDto> EnrolledCourses);