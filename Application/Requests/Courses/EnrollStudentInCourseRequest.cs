using Application.Responses.Courses;
using MediatR;

namespace Application.Requests.Courses;

public record EnrollStudentInCourseRequest (int CourseId, int StudentId) : IRequest<EnrollStudentInCourseResponse>;