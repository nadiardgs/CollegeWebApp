using Application.Features.Courses.Responses;

namespace Application.Features.Students.Responses;

public record GetStudentByIdResponse(int Id, string Name, List<CourseDto> EnrolledCourses);